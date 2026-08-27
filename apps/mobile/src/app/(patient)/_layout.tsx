import { Tabs } from 'expo-router';

export default function PatientLayout() {
  return (
    <Tabs screenOptions={{ tabBarActiveTintColor: '#28a745' }}>
      <Tabs.Screen
        name="dashboard"
        options={{
          title: 'Dashboard',
          headerTitle: 'My Health',
        }}
      />
      <Tabs.Screen
        name="search"
        options={{
          title: 'Find Doctor',
          headerTitle: 'Search Doctors',
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
