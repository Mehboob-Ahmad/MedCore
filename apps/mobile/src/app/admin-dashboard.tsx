import { useState, useEffect } from 'react';
import { StyleSheet, Text, View, ScrollView, Pressable, TextInput, ActivityIndicator, Alert } from 'react-native';
import { useRouter } from 'expo-router';
import { authService, adminService } from '../services/api';

export default function AdminDashboard() {
  const router = useRouter();
  
  const [stats, setStats] = useState({
    totalUsers: 0,
    totalDoctors: 0,
    totalPatients: 0,
    monthlyActive: 0,
  });
  
  const [statsLoading, setStatsLoading] = useState(true);
  const [inviteEmail, setInviteEmail] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadStats();
  }, []);

  const loadStats = async () => {
    try {
      setStatsLoading(true);
      const response = await adminService.getStats();
      if (response.success) {
        setStats(response.data);
      }
    } catch (error) {
      console.error('Failed to load stats', error);
      // Optional: keep zeros or show error alert
    } finally {
      setStatsLoading(false);
    }
  };

  const handleInviteAdmin = async () => {
    if (!inviteEmail) {
      Alert.alert('Error', 'Please enter an email address.');
      return;
    }

    try {
      setLoading(true);
      await authService.inviteAdmin({ email: inviteEmail });
      Alert.alert('Success', `Admin invitation sent to ${inviteEmail}.`);
      setInviteEmail(''); // clear input on success
    } catch (error: any) {
      const msg = error.response?.data?.message || error.message || 'Failed to send invitation.';
      Alert.alert('Error', msg);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    // Note: clear token here in real app
    router.replace('/');
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <View style={styles.header}>
        <Text style={styles.headerTitle}>Admin Overview</Text>
        <Pressable onPress={handleLogout} style={styles.logoutButton}>
          <Text style={styles.logoutText}>Log Out</Text>
        </Pressable>
      </View>

      {/* Stats Section */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Monthly Status</Text>
        {statsLoading ? (
          <View style={{ padding: 20, alignItems: 'center' }}>
            <ActivityIndicator size="large" color="#000" />
          </View>
        ) : (
          <View style={styles.statsGrid}>
            <View style={styles.statCard}>
              <Text style={styles.statNumber}>{stats.totalUsers}</Text>
              <Text style={styles.statLabel}>Total Users</Text>
            </View>
            <View style={styles.statCard}>
              <Text style={styles.statNumber}>{stats.monthlyActive}</Text>
              <Text style={styles.statLabel}>Monthly Active</Text>
            </View>
            <View style={[styles.statCard, styles.doctorCard]}>
              <Text style={[styles.statNumber, {color: '#fff'}]}>{stats.totalDoctors}</Text>
              <Text style={[styles.statLabel, {color: '#e0e0e0'}]}>Total Doctors</Text>
            </View>
            <View style={[styles.statCard, styles.patientCard]}>
              <Text style={[styles.statNumber, {color: '#fff'}]}>{stats.totalPatients}</Text>
              <Text style={[styles.statLabel, {color: '#e0e0e0'}]}>Total Patients</Text>
            </View>
          </View>
        )}
      </View>

      {/* Invite Admin Section */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>System Administration</Text>
        <View style={styles.inviteCard}>
          <Text style={styles.inviteTitle}>Invite New Super Admin</Text>
          <Text style={styles.inviteDesc}>
            Enter the email address of the person you want to invite. They will receive an email with instructions and their temporary password.
          </Text>
          
          <TextInput 
            style={styles.input} 
            placeholder="admin@example.com"
            keyboardType="email-address"
            autoCapitalize="none"
            value={inviteEmail}
            onChangeText={setInviteEmail}
          />

          <Pressable style={styles.inviteButton} onPress={handleInviteAdmin} disabled={loading}>
            {loading ? <ActivityIndicator color="#fff" /> : <Text style={styles.inviteButtonText}>Send Invitation</Text>}
          </Pressable>
        </View>
      </View>

    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f4f6f8',
  },
  content: {
    padding: 20,
    paddingBottom: 40,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 25,
    marginTop: 10,
  },
  headerTitle: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#1a1a1a',
  },
  logoutButton: {
    paddingVertical: 8,
    paddingHorizontal: 15,
    backgroundColor: '#ffe5e5',
    borderRadius: 20,
  },
  logoutText: {
    color: '#dc3545',
    fontWeight: 'bold',
    fontSize: 14,
  },
  section: {
    marginBottom: 30,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 15,
  },
  statsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-between',
  },
  statCard: {
    width: '48%',
    backgroundColor: '#fff',
    padding: 20,
    borderRadius: 12,
    marginBottom: 15,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 5,
    elevation: 3,
  },
  doctorCard: {
    backgroundColor: '#007bff',
  },
  patientCard: {
    backgroundColor: '#28a745',
  },
  statNumber: {
    fontSize: 26,
    fontWeight: 'bold',
    color: '#222',
    marginBottom: 5,
  },
  statLabel: {
    fontSize: 14,
    color: '#666',
    fontWeight: '500',
  },
  inviteCard: {
    backgroundColor: '#fff',
    padding: 20,
    borderRadius: 12,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 5,
    elevation: 3,
  },
  inviteTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#222',
    marginBottom: 8,
  },
  inviteDesc: {
    fontSize: 14,
    color: '#666',
    marginBottom: 20,
    lineHeight: 20,
  },
  input: {
    borderWidth: 1,
    borderColor: '#e0e0e0',
    backgroundColor: '#fafafa',
    borderRadius: 8,
    padding: 15,
    fontSize: 16,
    marginBottom: 15,
  },
  inviteButton: {
    backgroundColor: '#000',
    paddingVertical: 15,
    borderRadius: 8,
    alignItems: 'center',
  },
  inviteButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: 'bold',
  },
});
