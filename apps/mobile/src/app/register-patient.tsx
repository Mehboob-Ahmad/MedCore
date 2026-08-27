import { useState } from 'react';
import { StyleSheet, Text, View, TextInput, ScrollView, Pressable, ActivityIndicator, Alert } from 'react-native';
import { useRouter } from 'expo-router';
import { authService } from '../services/api';

export default function RegisterPatient() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);

  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    password: '',
    confirmPassword: '',
  });
  const [acceptTerms, setAcceptTerms] = useState(false);

  const handleRegister = async () => {
    // Basic validation
    if (!formData.firstName || !formData.lastName || !formData.email || !formData.password) {
      Alert.alert('Error', 'Please fill in all required fields.');
      return;
    }
    if (formData.password !== formData.confirmPassword) {
      Alert.alert('Error', 'Passwords do not match.');
      return;
    }
    if (!acceptTerms) {
      Alert.alert('Error', 'You must accept the terms and conditions.');
      return;
    }

    try {
      setLoading(true);
      const payload = {
        ...formData,
        acceptTerms: acceptTerms,
      };

      await authService.registerPatient(payload);
      Alert.alert('Success', 'Registration successful! Please check your email to verify your account.');
      router.back();
    } catch (error: any) {
      const msg = error.response?.data?.message || error.message || 'Registration failed';
      Alert.alert('Registration Failed', msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.title}>Patient Registration</Text>

      <View style={styles.inputGroup}>
        <Text style={styles.label}>First Name *</Text>
        <TextInput style={styles.input} value={formData.firstName} onChangeText={(t) => setFormData({...formData, firstName: t})} />
      </View>
      <View style={styles.inputGroup}>
        <Text style={styles.label}>Last Name *</Text>
        <TextInput style={styles.input} value={formData.lastName} onChangeText={(t) => setFormData({...formData, lastName: t})} />
      </View>
      <View style={styles.inputGroup}>
        <Text style={styles.label}>Email *</Text>
        <TextInput style={styles.input} keyboardType="email-address" autoCapitalize="none" value={formData.email} onChangeText={(t) => setFormData({...formData, email: t})} />
      </View>
      <View style={styles.inputGroup}>
        <Text style={styles.label}>Phone Number *</Text>
        <TextInput style={styles.input} keyboardType="phone-pad" value={formData.phoneNumber} onChangeText={(t) => setFormData({...formData, phoneNumber: t})} />
      </View>
      <View style={styles.inputGroup}>
        <Text style={styles.label}>Password *</Text>
        <TextInput style={styles.input} secureTextEntry value={formData.password} onChangeText={(t) => setFormData({...formData, password: t})} />
      </View>
      <View style={styles.inputGroup}>
        <Text style={styles.label}>Confirm Password *</Text>
        <TextInput style={styles.input} secureTextEntry value={formData.confirmPassword} onChangeText={(t) => setFormData({...formData, confirmPassword: t})} />
      </View>

      <Pressable style={styles.checkboxContainer} onPress={() => setAcceptTerms(!acceptTerms)}>
        <View style={[styles.checkbox, acceptTerms && styles.checkboxSelected]} />
        <Text style={styles.checkboxLabel}>I accept the Terms and Conditions</Text>
      </Pressable>

      <Pressable style={styles.button} onPress={handleRegister} disabled={loading}>
        {loading ? <ActivityIndicator color="#fff" /> : <Text style={styles.buttonText}>Register</Text>}
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
  checkboxContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 10,
    marginBottom: 30,
  },
  checkbox: {
    width: 20,
    height: 20,
    borderWidth: 1,
    borderColor: '#007bff',
    borderRadius: 4,
    marginRight: 10,
  },
  checkboxSelected: {
    backgroundColor: '#007bff',
  },
  checkboxLabel: {
    fontSize: 16,
  },
  button: {
    backgroundColor: '#007bff',
    paddingVertical: 15,
    borderRadius: 8,
    alignItems: 'center',
  },
  buttonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
  },
});
