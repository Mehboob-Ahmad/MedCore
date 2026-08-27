import { useState } from 'react';
import { StyleSheet, Text, View, ScrollView, Switch, Pressable, TextInput, ActivityIndicator, Alert } from 'react-native';
import { doctorService } from '../../services/api';

const DAYS_OF_WEEK = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

export default function DoctorSchedule() {
  const [loading, setLoading] = useState(false);
  
  // Default 9-5 schedule for weekdays, off on weekends
  const [schedule, setSchedule] = useState(
    DAYS_OF_WEEK.map((day, index) => ({
      dayOfWeek: index,
      name: day,
      isAvailable: index >= 1 && index <= 5, // Mon-Fri true
      startTime: '09:00',
      endTime: '17:00'
    }))
  );

  const toggleDay = (index: number) => {
    const newSchedule = [...schedule];
    newSchedule[index].isAvailable = !newSchedule[index].isAvailable;
    setSchedule(newSchedule);
  };

  const updateTime = (index: number, field: 'startTime' | 'endTime', value: string) => {
    const newSchedule = [...schedule];
    newSchedule[index][field] = value;
    setSchedule(newSchedule);
  };

  const handleSave = async () => {
    try {
      setLoading(true);
      // Backend expects Days array
      const payload = {
        days: schedule.map(d => ({
          dayOfWeek: d.dayOfWeek,
          startTime: d.startTime,
          endTime: d.endTime,
          isAvailable: d.isAvailable
        }))
      };

      await doctorService.configureAvailability(payload);
      Alert.alert('Success', 'Availability schedule updated successfully!');
    } catch (error: any) {
      const msg = error.response?.data?.message || error.message || 'Failed to save schedule';
      Alert.alert('Error', msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.title}>Weekly Schedule</Text>
      <Text style={styles.subtitle}>Configure your available hours for appointments.</Text>

      {schedule.map((day, index) => (
        <View key={day.dayOfWeek} style={styles.dayCard}>
          <View style={styles.dayHeader}>
            <Text style={styles.dayName}>{day.name}</Text>
            <Switch
              value={day.isAvailable}
              onValueChange={() => toggleDay(index)}
              trackColor={{ false: '#767577', true: '#28a745' }}
              thumbColor="#f4f3f4"
            />
          </View>
          
          {day.isAvailable && (
            <View style={styles.timeInputs}>
              <View style={styles.timeGroup}>
                <Text style={styles.timeLabel}>Start (HH:MM)</Text>
                <TextInput
                  style={styles.input}
                  value={day.startTime}
                  onChangeText={(val) => updateTime(index, 'startTime', val)}
                  placeholder="09:00"
                />
              </View>
              <Text style={styles.toText}>to</Text>
              <View style={styles.timeGroup}>
                <Text style={styles.timeLabel}>End (HH:MM)</Text>
                <TextInput
                  style={styles.input}
                  value={day.endTime}
                  onChangeText={(val) => updateTime(index, 'endTime', val)}
                  placeholder="17:00"
                />
              </View>
            </View>
          )}
        </View>
      ))}

      <Pressable style={styles.saveButton} onPress={handleSave} disabled={loading}>
        {loading ? <ActivityIndicator color="#fff" /> : <Text style={styles.saveButtonText}>Save Schedule</Text>}
      </Pressable>
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
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 5,
    color: '#333',
  },
  subtitle: {
    fontSize: 14,
    color: '#666',
    marginBottom: 20,
  },
  dayCard: {
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
  dayHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  dayName: {
    fontSize: 16,
    fontWeight: '600',
    color: '#222',
  },
  timeInputs: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginTop: 15,
    paddingTop: 15,
    borderTopWidth: 1,
    borderTopColor: '#f0f0f0',
  },
  timeGroup: {
    flex: 1,
  },
  timeLabel: {
    fontSize: 12,
    color: '#777',
    marginBottom: 5,
  },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 8,
    padding: 10,
    fontSize: 16,
    backgroundColor: '#fafafa',
  },
  toText: {
    marginHorizontal: 15,
    marginTop: 15,
    color: '#666',
    fontWeight: '500',
  },
  saveButton: {
    backgroundColor: '#007bff',
    paddingVertical: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 20,
  },
  saveButtonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
  },
});
