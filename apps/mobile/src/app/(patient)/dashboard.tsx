import { useState, useEffect } from 'react';
import { StyleSheet, Text, View, ScrollView, ActivityIndicator, Pressable, Alert } from 'react-native';
import { useRouter } from 'expo-router';
import * as Clipboard from 'expo-clipboard';
import { patientService } from '../../services/api';

export default function PatientDashboard() {
  const router = useRouter();
  const [loading, setLoading] = useState(true);
  const [data, setData] = useState<any>(null);

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    try {
      setLoading(true);
      const response = await patientService.getDashboard();
      if (response.success) {
        setData(response.data);
      }
    } catch (error: any) {
      Alert.alert('Error', 'Failed to fetch dashboard data from live database');
      setData({ upcomingAppointments: [] });
    } finally {
      setLoading(false);
    }
  };

  const handleCopy = async (text: string) => {
    await Clipboard.setStringAsync(text);
    Alert.alert('Copied', 'Account number copied to clipboard');
  };

  const handleLogout = () => {
    router.replace('/');
  };

  if (loading) {
    return (
      <View style={styles.loadingContainer}>
        <ActivityIndicator size="large" color="#28a745" />
      </View>
    );
  }

  const { patientSummary, quickStats, upcomingAppointments } = data || {};

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <View style={styles.header}>
        <View>
          <Text style={styles.greeting}>Hello,</Text>
          <Text style={styles.title}>{patientSummary?.firstName || 'Patient'}</Text>
        </View>
        <Pressable onPress={handleLogout} style={styles.logoutButton}>
          <Text style={styles.logoutText}>Log Out</Text>
        </Pressable>
      </View>

      <View style={styles.progressContainer}>
        <Text style={styles.progressText}>Profile Completion: {patientSummary?.profileCompletionPct || 0}%</Text>
        <View style={styles.progressBarBg}>
          <View style={[styles.progressBarFill, { width: `${patientSummary?.profileCompletionPct || 0}%` }]} />
        </View>
      </View>

      <View style={styles.statsGrid}>
        <View style={styles.statCard}>
          <Text style={styles.statNumber}>{quickStats?.activePrescriptionsCount || 0}</Text>
          <Text style={styles.statLabel}>Active Scripts</Text>
        </View>
        <View style={styles.statCard}>
          <Text style={styles.statNumber}>{quickStats?.unreadMessagesCount || 0}</Text>
          <Text style={styles.statLabel}>Unread Msgs</Text>
        </View>
      </View>

      <View style={styles.section}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>Upcoming Appointments</Text>
          <Pressable onPress={() => router.push('/(patient)/search')}>
            <Text style={styles.seeAllText}>Book New</Text>
          </Pressable>
        </View>
        
        {upcomingAppointments?.length > 0 ? (
          upcomingAppointments.map((appt: any) => (
            <View key={appt.appointmentId} style={styles.apptCard}>
              <View style={styles.apptCardHeader}>
                <View>
                  <Text style={styles.apptDate}>{new Date(appt.scheduledDate).toLocaleDateString()} at {new Date(appt.scheduledDate).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'})}</Text>
                  <Text style={styles.apptName}>{appt.doctorName}</Text>
                  <Text style={styles.apptSpecialty}>{appt.specialty} • {appt.type}</Text>
                </View>
                <View style={styles.statusBadge}>
                  <Text style={styles.statusText}>{appt.status}</Text>
                </View>
              </View>
              {appt.paymentStatus === 'Pending' && appt.paymentMethods?.length > 0 && (
                <View style={styles.paymentContainer}>
                  <Text style={styles.paymentNotice}>Payment Pending</Text>
                  {appt.paymentMethods.map((pm: any, idx: number) => (
                    <View key={idx} style={styles.paymentMethodCard}>
                      <Text style={styles.pmTitle}>{pm.paymentProvider} ({pm.paymentMethodType})</Text>
                      <Text style={styles.pmAccountName}>{pm.accountTitle}</Text>
                      <View style={styles.pmAccountRow}>
                        <Text style={styles.pmAccountNumber}>{pm.maskedAccountNumber}</Text>
                        <Pressable style={styles.copyButton} onPress={() => handleCopy(pm.accountNumber)}>
                          <Text style={styles.copyButtonText}>Copy</Text>
                        </Pressable>
                      </View>
                    </View>
                  ))}
                </View>
              )}
            </View>
          ))
        ) : (
          <View style={styles.emptyCard}>
            <Text style={styles.emptyText}>No upcoming appointments.</Text>
            <Pressable style={styles.bookButton} onPress={() => router.push('/(patient)/search')}>
              <Text style={styles.bookButtonText}>Find a Doctor</Text>
            </Pressable>
          </View>
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
    backgroundColor: '#f8f9fa',
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
  greeting: {
    fontSize: 16,
    color: '#666',
  },
  title: {
    fontSize: 26,
    fontWeight: 'bold',
    color: '#222',
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
  progressContainer: {
    backgroundColor: '#fff',
    padding: 15,
    borderRadius: 12,
    marginBottom: 20,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 3,
    elevation: 2,
  },
  progressText: {
    fontSize: 14,
    fontWeight: '600',
    color: '#444',
    marginBottom: 8,
  },
  progressBarBg: {
    height: 8,
    backgroundColor: '#e9ecef',
    borderRadius: 4,
    overflow: 'hidden',
  },
  progressBarFill: {
    height: '100%',
    backgroundColor: '#28a745',
  },
  statsGrid: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 25,
  },
  statCard: {
    width: '48%',
    backgroundColor: '#fff',
    padding: 15,
    borderRadius: 12,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 3,
    elevation: 2,
  },
  statNumber: {
    fontSize: 24,
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
  sectionHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 15,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#333',
  },
  seeAllText: {
    color: '#28a745',
    fontWeight: '600',
  },
  apptCard: {
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
  apptCardHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  paymentContainer: {
    marginTop: 15,
    paddingTop: 15,
    borderTopWidth: 1,
    borderTopColor: '#f0f0f0',
  },
  paymentNotice: {
    fontSize: 14,
    fontWeight: 'bold',
    color: '#dc3545',
    marginBottom: 10,
  },
  paymentMethodCard: {
    backgroundColor: '#f8f9fa',
    padding: 10,
    borderRadius: 8,
    marginBottom: 8,
    borderWidth: 1,
    borderColor: '#e9ecef',
  },
  pmTitle: {
    fontSize: 14,
    fontWeight: '600',
    color: '#333',
  },
  pmAccountName: {
    fontSize: 12,
    color: '#666',
    marginBottom: 5,
  },
  pmAccountRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#fff',
    padding: 8,
    borderRadius: 6,
    borderWidth: 1,
    borderColor: '#ddd',
  },
  pmAccountNumber: {
    fontSize: 14,
    fontFamily: 'monospace',
    fontWeight: '600',
    color: '#333',
  },
  copyButton: {
    backgroundColor: '#e9ecef',
    paddingHorizontal: 12,
    paddingVertical: 4,
    borderRadius: 4,
  },
  copyButtonText: {
    fontSize: 12,
    fontWeight: 'bold',
    color: '#495057',
  },
  apptDate: {
    fontSize: 13,
    color: '#28a745',
    fontWeight: 'bold',
    marginBottom: 4,
  },
  apptName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#222',
    marginBottom: 2,
  },
  apptSpecialty: {
    fontSize: 13,
    color: '#666',
  },
  statusBadge: {
    backgroundColor: '#e6f4ea',
    paddingHorizontal: 10,
    paddingVertical: 5,
    borderRadius: 12,
  },
  statusText: {
    color: '#1e7e34',
    fontSize: 12,
    fontWeight: 'bold',
  },
  emptyCard: {
    backgroundColor: '#fff',
    padding: 30,
    borderRadius: 12,
    alignItems: 'center',
  },
  emptyText: {
    fontSize: 15,
    color: '#777',
    marginBottom: 15,
  },
  bookButton: {
    backgroundColor: '#28a745',
    paddingVertical: 10,
    paddingHorizontal: 20,
    borderRadius: 20,
  },
  bookButtonText: {
    color: '#fff',
    fontWeight: 'bold',
  }
});
