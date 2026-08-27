import { Stack, useRouter } from 'expo-router';
import { useEffect } from 'react';
import { usePushNotifications } from '../hooks/usePushNotifications';

export default function Layout() {
  const { expoPushToken, notification } = usePushNotifications();
  const router = useRouter();

  useEffect(() => {
    if (notification) {
      const url = notification.request.content.data?.url as string;
      if (url) {
        router.push(url);
      }
    }
  }, [notification]);

  return (
    <Stack>
      <Stack.Screen name="index" options={{ title: 'Home' }} />
      <Stack.Screen name="role-selection" options={{ title: 'Select Role' }} />
      <Stack.Screen name="register-doctor" options={{ title: 'Doctor Registration' }} />
      <Stack.Screen name="register-patient" options={{ title: 'Patient Registration' }} />
      <Stack.Screen name="login" options={{ title: 'Log In' }} />
      <Stack.Screen name="(patient)" options={{ headerShown: false }} />
      <Stack.Screen name="(doctor)" options={{ headerShown: false }} />
      <Stack.Screen name="admin-dashboard" options={{ title: 'Admin Dashboard', headerLeft: () => null }} />
    </Stack>
  );
}
