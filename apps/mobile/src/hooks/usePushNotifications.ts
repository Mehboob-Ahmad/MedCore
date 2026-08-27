import { useState, useEffect, useRef } from 'react';
import { Platform } from 'react-native';
import * as Device from 'expo-device';
import type * as NotificationsType from 'expo-notifications';
import Constants from 'expo-constants';
import { authService } from '../services/api';

export interface PushNotificationState {
  expoPushToken?: string;
  notification?: NotificationsType.Notification;
}

export const usePushNotifications = (): PushNotificationState => {
  const [expoPushToken, setExpoPushToken] = useState<string | undefined>();
  const [notification, setNotification] = useState<NotificationsType.Notification | undefined>();
  const notificationListener = useRef<any>(null);
  const responseListener = useRef<any>(null);

  async function registerForPushNotificationsAsync() {
    let token;

    if (Constants.appOwnership === 'expo') {
      console.log('Push notifications are not supported in Expo Go (SDK 53+). Skipping registration.');
      return undefined;
    }

    try {
      const Notifications = require('expo-notifications');

      if (Platform.OS === 'android') {
        await Notifications.setNotificationChannelAsync('default', {
          name: 'default',
          importance: Notifications.AndroidImportance.MAX,
          vibrationPattern: [0, 250, 250, 250],
          lightColor: '#FF231F7C',
        });
      }

      if (Device.isDevice) {
        const { status: existingStatus } = await Notifications.getPermissionsAsync();
        let finalStatus = existingStatus;
        if (existingStatus !== 'granted') {
          const { status } = await Notifications.requestPermissionsAsync();
          finalStatus = status;
        }
        if (finalStatus !== 'granted') {
          console.log('Failed to get push token for push notification!');
          return;
        }
        
        const projectId = Constants?.expoConfig?.extra?.eas?.projectId ?? Constants?.easConfig?.projectId;
        
        try {
            token = (await Notifications.getExpoPushTokenAsync({ projectId })).data;
            console.log('Expo Push Token:', token);
            
            // Send token to backend
            try {
                await authService.updatePushToken(token);
                console.log('Push token saved to backend successfully.');
            } catch (apiError) {
                console.error('Error saving push token to backend:', apiError);
            }
        } catch (e) {
            console.error('Error getting push token', e);
        }
      } else {
        console.log('Must use physical device for Push Notifications');
      }
    } catch (e) {
      console.log('expo-notifications not available or crashed', e);
    }

    return token;
  }

  useEffect(() => {
    registerForPushNotificationsAsync().then(token => setExpoPushToken(token));

    if (Constants.appOwnership === 'expo') {
      return; // Skip listeners in Expo Go
    }

    try {
      const Notifications = require('expo-notifications');

      notificationListener.current = Notifications.addNotificationReceivedListener((notif: NotificationsType.Notification) => {
        setNotification(notif);
      });

      responseListener.current = Notifications.addNotificationResponseReceivedListener((response: any) => {
        console.log('Notification tapped:', response);
      });
    } catch (e) {
      console.log('Failed to attach notification listeners', e);
    }

    return () => {
      if (notificationListener.current) {
        notificationListener.current.remove();
      }
      if (responseListener.current) {
        responseListener.current.remove();
      }
    };
  }, []);

  return { expoPushToken, notification };
};
