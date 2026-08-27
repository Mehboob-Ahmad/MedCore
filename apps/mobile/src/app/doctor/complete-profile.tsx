import { useState } from 'react';
import { StyleSheet, Text, View, TextInput, ScrollView, Pressable, ActivityIndicator, Alert } from 'react-native';
import { useRouter } from 'expo-router';

export default function CompleteProfile() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);

  const [formData, setFormData] = useState({
    specialization: '',
    contactNumber: '',
    licensingAuthority: '',
    clinicName: '',
    availabilityHours: '',
    consultationFee: '',
  });

  const handleSave = async () => {
    // Basic validation
    if (!formData.specialization || !formData.clinicName || !formData.availabilityHours) {
      Alert.alert('Error', 'Please fill in the required fields.');
      return;
    }

    try {
      setLoading(true);
      // Mock API Call: await authService.completeDoctorProfile(formData);
      
      Alert.alert('Success', 'Profile completed successfully!');
      router.push('/doctor/dashboard');
    } catch (error: any) {
      Alert.alert('Error', 'Failed to save profile');
    } finally {
      setLoading(false);
    }
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.title}>Complete Your Profile</Text>
      <Text style={styles.subtitle}>Tell us more about your practice to start accepting patients.</Text>

      <View style={styles.inputGroup}>
        <Text style={styles.label}>Specialization *</Text>
        <TextInput style={styles.input} placeholder="e.g. Cardiology" value={formData.specialization} onChangeText={(t) => setFormData({...formData, specialization: t})} />
      </View>

      <View style={styles.inputGroup}>
        <Text style={styles.label}>Contact Number *</Text>
        <TextInput style={styles.input} keyboardType="phone-pad" placeholder="+1 (555) 000-0000" value={formData.contactNumber} onChangeText={(t) => setFormData({...formData, contactNumber: t})} />
      </View>

      <View style={styles.inputGroup}>
        <Text style={styles.label}>Licensing Authority *</Text>
        <TextInput style={styles.input} placeholder="e.g. Medical Council" value={formData.licensingAuthority} onChangeText={(t) => setFormData({...formData, licensingAuthority: t})} />
      </View>

      <View style={styles.inputGroup}>
        <Text style={styles.label}>Clinic / Hospital Name *</Text>
        <TextInput style={styles.input} placeholder="City General Hospital" value={formData.clinicName} onChangeText={(t) => setFormData({...formData, clinicName: t})} />
      </View>
      
      <View style={styles.inputGroup}>
        <Text style={styles.label}>Availability Hours *</Text>
        <TextInput style={styles.input} placeholder="Mon-Fri, 9:00 AM - 5:00 PM" value={formData.availabilityHours} onChangeText={(t) => setFormData({...formData, availabilityHours: t})} />
      </View>

      <View style={styles.inputGroup}>
        <Text style={styles.label}>Consultation Fee ($) *</Text>
        <TextInput style={styles.input} keyboardType="decimal-pad" placeholder="100" value={formData.consultationFee} onChangeText={(t) => setFormData({...formData, consultationFee: t})} />
      </View>

      <Pressable style={styles.button} onPress={handleSave} disabled={loading}>
        {loading ? <ActivityIndicator color="#fff" /> : <Text style={styles.buttonText}>Save Profile</Text>}
      </Pressable>

      <Pressable style={styles.skipButton} onPress={() => router.push('/doctor/dashboard')}>
        <Text style={styles.skipButtonText}>Skip for now</Text>
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
    fontSize: 28,
    fontWeight: 'bold',
    marginBottom: 5,
    color: '#007bff',
  },
  subtitle: {
    fontSize: 16,
    color: '#666',
    marginBottom: 30,
  },
  inputGroup: {
    marginBottom: 15,
  },
  label: {
    fontSize: 14,
    color: '#333',
    marginBottom: 5,
    fontWeight: '600',
  },
  input: {
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 8,
    padding: 12,
    fontSize: 16,
  },
  button: {
    backgroundColor: '#007bff',
    paddingVertical: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 10,
  },
  buttonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
  },
  skipButton: {
    paddingVertical: 15,
    alignItems: 'center',
    marginTop: 5,
  },
  skipButtonText: {
    color: '#666',
    fontSize: 16,
  }
});
