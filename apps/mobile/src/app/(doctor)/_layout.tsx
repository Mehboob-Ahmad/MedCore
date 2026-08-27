import { Tabs } from 'expo-router';

export default function DoctorLayout() {
  return (
    <Tabs screenOptions={{ tabBarActiveTintColor: '#007bff' }}>
      <Tabs.Screen
        name="dashboard"
        options={{
          title: 'Dashboard',
          headerTitle: 'Doctor Dashboard',
        }}
      />
      <Tabs.Screen
        name="schedule"
        options={{
          title: 'Schedule',
          headerTitle: 'My Availability',
        }}
      />
      <Tabs.Screen
        name="profile"
        options={{
          title: 'Profile',
          headerTitle: 'My Profile',
        }}
      />
      <Tabs.Screen
        name="messages"
        options={{
          title: 'Messages',
          headerTitle: 'Inbox',
        }}
      />
    </Tabs>
  );
}
