import { useState, useEffect } from 'react';
import { StyleSheet, Text, View, ScrollView, ActivityIndicator, Pressable, Alert } from 'react-native';
import { useRouter } from 'expo-router';
import { doctorService, appointmentService } from '../../services/api';

export default function DoctorDashboard() {
  const router = useRouter();
  const [loading, setLoading] = useState(true);
  const [data, setData] = useState<any>(null);

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    try {
      setLoading(true);
      const response = await doctorService.getDashboard();
      if (response.success) {
        setData(response.data);
      }
    } catch (error: any) {
      // Temporary fallback for dev/testing if auth token is missing
      Alert.alert('Notice', 'Using offline mock data (Failed to fetch API)');
      setData({
        totalPatients: 120,
        todayAppointmentsCount: 4,
        pendingReports: 2,
        revenueThisMonth: 4500,
        todayAppointments: [
          { id: '1', patientName: 'John Doe', time: '10:00 AM', status: 'Scheduled' },
          { id: '2', patientName: 'Jane Smith', time: '11:30 AM', status: 'In Progress' }
        ],
        recentConsultations: []
      });
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    router.replace('/');
  };

  const handleUpdateStatus = async (id: string, status: string) => {
    try {
      await appointmentService.updateAppointmentStatus(id, status);
      // Reload dashboard after update
      loadDashboard();
      Alert.alert('Success', `Appointment ${status.toLowerCase()}`);
    } catch (error: any) {
      Alert.alert('Error', error.response?.data?.message || 'Failed to update status');
    }
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#007bff" />
      </View>
    );
  }

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <View style={styles.header}>
        <Text style={styles.title}>Overview</Text>
        <Pressable onPress={handleLogout} style={styles.logoutButton}>
          <Text style={styles.logoutText}>Log Out</Text>
        </Pressable>
      </View>

      <View style={styles.statsGrid}>
        <View style={styles.statCard}>
          <Text style={styles.statNumber}>{data?.totalPatients || 0}</Text>
          <Text style={styles.statLabel}>Total Patients</Text>
        </View>
        <View style={styles.statCard}>
          <Text style={styles.statNumber}>{data?.todayAppointmentsCount || 0}</Text>
          <Text style={styles.statLabel}>Today's Appts</Text>
        </View>
        <View style={styles.statCard}>
          <Text style={styles.statNumber}>{data?.pendingReports || 0}</Text>
          <Text style={styles.statLabel}>Pending Reports</Text>
        </View>
        <View style={[styles.statCard, { backgroundColor: '#e6f7ff' }]}>
          <Text style={[styles.statNumber, { color: '#0050b3' }]}>${data?.revenueThisMonth || 0}</Text>
          <Text style={[styles.statLabel, { color: '#0050b3' }]}>Monthly Revenue</Text>
        </View>
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Today's Schedule</Text>
        {data?.todayAppointments?.length > 0 ? (
          data.todayAppointments.map((appt: any) => (
            <View key={appt.id} style={styles.apptCard}>
              <View style={{ flex: 1 }}>
                <Text style={styles.apptTime}>{appt.time}</Text>
                <Text style={styles.apptName}>{appt.patientName}</Text>
              </View>
              
              {appt.status === 'Pending' ? (
                <View style={styles.actionButtons}>
                  <Pressable style={styles.rejectBtn} onPress={() => handleUpdateStatus(appt.id, 'Cancelled')}>
                    <Text style={styles.rejectText}>Reject</Text>
                  </Pressable>
                  <Pressable style={styles.approveBtn} onPress={() => handleUpdateStatus(appt.id, 'Confirmed')}>
                    <Text style={styles.approveText}>Approve</Text>
                  </Pressable>
                </View>
              ) : (
                <View style={[styles.statusBadge, appt.status === 'Confirmed' && {backgroundColor: '#e6f4ea'}]}>
                  <Text style={[styles.statusText, appt.status === 'Confirmed' && {color: '#1e7e34'}]}>{appt.status}</Text>
                </View>
              )}
            </View>
          ))
        ) : (
          <Text style={styles.emptyText}>No appointments scheduled for today.</Text>
        )}
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  loadingContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
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
    marginBottom: 20,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
  },
  logoutButton: {
    paddingVertical: 6,
    paddingHorizontal: 12,
    backgroundColor: '#ffe5e5',
    borderRadius: 15,
  },
  logoutText: {
    color: '#dc3545',
    fontWeight: 'bold',
    fontSize: 12,
  },
  statsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-between',
    marginBottom: 25,
  },
  statCard: {
    width: '48%',
    backgroundColor: '#fff',
    padding: 15,
    borderRadius: 12,
    marginBottom: 15,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 3,
    elevation: 2,
  },
  statNumber: {
    fontSize: 22,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 4,
  },
  statLabel: {
    fontSize: 13,
    color: '#666',
    fontWeight: '500',
  },
  section: {
    marginBottom: 25,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 15,
    color: '#333',
  },
  apptCard: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#fff',
    padding: 15,
    borderRadius: 12,
    marginBottom: 10,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 3,
    elevation: 2,
  },
  apptTime: {
    fontSize: 14,
    color: '#007bff',
    fontWeight: 'bold',
    marginBottom: 4,
  },
  apptName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#222',
  },
  statusBadge: {
    backgroundColor: '#e6f7ff',
    paddingHorizontal: 10,
    paddingVertical: 5,
    borderRadius: 12,
  },
  statusText: {
    color: '#0050b3',
    fontSize: 12,
    fontWeight: 'bold',
  },
  emptyText: {
    fontSize: 15,
    color: '#777',
    fontStyle: 'italic',
  },
  actionButtons: {
    flexDirection: 'row',
    gap: 10,
  },
  approveBtn: {
    backgroundColor: '#28a745',
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 8,
  },
  approveText: {
    color: '#fff',
    fontWeight: 'bold',
    fontSize: 12,
  },
  rejectBtn: {
    backgroundColor: '#ffe5e5',
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 8,
  },
  rejectText: {
    color: '#dc3545',
    fontWeight: 'bold',
    fontSize: 12,
  }
});
