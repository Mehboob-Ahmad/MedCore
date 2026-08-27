import { useState, useEffect } from 'react';
import { StyleSheet, Text, View, ScrollView, Pressable, TextInput, ActivityIndicator, Alert } from 'react-native';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { appointmentService } from '../../../services/api';

export default function BookAppointment() {
  const { doctorId } = useLocalSearchParams();
  const router = useRouter();
  
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [slots, setSlots] = useState<any[]>([]);
  
  const [selectedDate, setSelectedDate] = useState(() => {
    const d = new Date();
    return d.toISOString().split('T')[0]; // YYYY-MM-DD
  });
  const [selectedSlot, setSelectedSlot] = useState<any>(null);
  const [bookingNote, setBookingNote] = useState('');

  useEffect(() => {
    if (doctorId) {
      loadSlots(selectedDate);
    }
  }, [doctorId, selectedDate]);

  const loadSlots = async (date: string) => {
    try {
      setLoading(true);
      setSelectedSlot(null);
      const response = await appointmentService.getAvailableSlots(doctorId as string, date);
      if (response.success) {
        setSlots(response.data || []);
      }
    } catch (error) {
      // Mock for development if offline
      setSlots([
        { slotId: '1', startTime: '2023-10-15T09:00:00', endTime: '2023-10-15T09:30:00', isAvailable: true },
        { slotId: '2', startTime: '2023-10-15T09:30:00', endTime: '2023-10-15T10:00:00', isAvailable: false },
        { slotId: '3', startTime: '2023-10-15T10:00:00', endTime: '2023-10-15T10:30:00', isAvailable: true },
        { slotId: '4', startTime: '2023-10-15T11:00:00', endTime: '2023-10-15T11:30:00', isAvailable: true },
      ]);
    } finally {
      setLoading(false);
    }
  };

  const handleBook = async () => {
    if (!selectedSlot) {
      Alert.alert('Error', 'Please select a time slot.');
      return;
    }

    try {
      setSubmitting(true);
      
      const payload = {
        doctorId: doctorId as string,
        scheduledDate: selectedSlot.startTime, 
        startTime: new Date(selectedSlot.startTime).toLocaleTimeString([], { hour12: false, hour: '2-digit', minute: '2-digit' }),
        bookingNote: bookingNote,
      };

      await appointmentService.bookAppointment(payload);
      Alert.alert('Success', 'Appointment request submitted successfully!');
      router.replace('/(patient)/dashboard');
    } catch (error: any) {
      const msg = error.response?.data?.message || error.message || 'Failed to book appointment';
      Alert.alert('Error', msg);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.title}>Book Appointment</Text>
      <Text style={styles.subtitle}>Select a date and time to see the doctor.</Text>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Select Date</Text>
        <TextInput
          style={styles.dateInput}
          value={selectedDate}
          onChangeText={setSelectedDate}
          placeholder="YYYY-MM-DD"
        />
        <Text style={styles.hint}>Format: YYYY-MM-DD. Modifying this instantly fetches slots.</Text>
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Available Time Slots</Text>
        {loading ? (
          <ActivityIndicator size="small" color="#28a745" style={{ marginVertical: 20 }} />
        ) : slots.length === 0 ? (
          <Text style={styles.noSlots}>No availability on this date.</Text>
        ) : (
          <View style={styles.slotsGrid}>
            {slots.map((slot) => {
              const timeString = new Date(slot.startTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
              const isSelected = selectedSlot?.slotId === slot.slotId;
              
              return (
                <Pressable
                  key={slot.slotId}
                  style={[
                    styles.slotCard,
                    !slot.isAvailable && styles.slotUnavailable,
                    isSelected && styles.slotSelected
                  ]}
                  disabled={!slot.isAvailable}
                  onPress={() => setSelectedSlot(slot)}
                >
                  <Text style={[
                    styles.slotTime,
                    !slot.isAvailable && styles.slotTimeUnavailable,
                    isSelected && styles.slotTimeSelected
                  ]}>
                    {timeString}
                  </Text>
                </Pressable>
              );
            })}
          </View>
        )}
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Notes for the Doctor (Optional)</Text>
        <TextInput
          style={[styles.input, styles.textArea]}
          multiline
          numberOfLines={4}
          placeholder="Briefly describe your symptoms or reason for visit..."
          value={bookingNote}
          onChangeText={setBookingNote}
        />
      </View>

      <Pressable 
        style={[styles.bookButton, (!selectedSlot || submitting) && styles.bookButtonDisabled]} 
        onPress={handleBook} 
        disabled={!selectedSlot || submitting}
      >
        {submitting ? <ActivityIndicator color="#fff" /> : <Text style={styles.bookButtonText}>Confirm Booking</Text>}
      </Pressable>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
  },
  content: {
    padding: 20,
    paddingBottom: 40,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 5,
  },
  subtitle: {
    fontSize: 14,
    color: '#666',
    marginBottom: 25,
  },
  section: {
    marginBottom: 25,
  },
  sectionTitle: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#222',
    marginBottom: 10,
  },
  dateInput: {
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 8,
    padding: 12,
    fontSize: 16,
    backgroundColor: '#fafafa',
  },
  hint: {
    fontSize: 12,
    color: '#888',
    marginTop: 5,
  },
  slotsGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: 10,
  },
  slotCard: {
    width: '30%',
    paddingVertical: 12,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#28a745',
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: 10,
  },
  slotUnavailable: {
    borderColor: '#e0e0e0',
    backgroundColor: '#f5f5f5',
  },
  slotSelected: {
    backgroundColor: '#28a745',
  },
  slotTime: {
    fontSize: 14,
    fontWeight: '600',
    color: '#28a745',
  },
  slotTimeUnavailable: {
    color: '#aaa',
  },
  slotTimeSelected: {
    color: '#fff',
  },
  noSlots: {
    fontSize: 14,
    color: '#777',
    fontStyle: 'italic',
  },
  input: {
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 8,
    padding: 12,
    fontSize: 16,
    backgroundColor: '#fafafa',
  },
  textArea: {
    height: 100,
    textAlignVertical: 'top',
  },
  bookButton: {
    backgroundColor: '#28a745',
    paddingVertical: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 10,
  },
  bookButtonDisabled: {
    backgroundColor: '#94d3a2',
  },
  bookButtonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
  },
});
