import { useState } from 'react';
import { StyleSheet, Text, View, TextInput, ScrollView, Pressable, ActivityIndicator, Alert } from 'react-native';
import { useRouter } from 'expo-router';
import { authService } from '../services/api';

export default function RegisterDoctor() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);

  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: '',
  });
  const [acceptTerms, setAcceptTerms] = useState(false);
  const [mbbsFile, setMbbsFile] = useState<string | null>(null);
  const [licenseFile, setLicenseFile] = useState<string | null>(null);

  const handlePickDocument = (setter: React.Dispatch<React.SetStateAction<string | null>>, name: string) => {
    // Mock document picker
    setter(`mock_${name.replace(/\s+/g, '_').toLowerCase()}.pdf`);
    Alert.alert('Document Selected', `${name} selected successfully.`);
  };

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
    if (!mbbsFile || !licenseFile) {
      Alert.alert('Error', 'Please upload both MBBS Degree and Doctor License.');
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
        mbbsDegreeFileId: "00000000-0000-0000-0000-000000000000", // Mock ID for now
        licenseFileId: "00000000-0000-0000-0000-000000000000", // Mock ID for now
      };

      await authService.registerDoctor(payload);
      
      // Navigate to complete profile
      router.push('/doctor/complete-profile');
    } catch (error: any) {
      const msg = error.response?.data?.message || error.message || 'Registration failed';
      Alert.alert('Registration Failed', msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Text style={styles.title}>Doctor Registration</Text>

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
        <Text style={styles.label}>Password *</Text>
        <TextInput style={styles.input} secureTextEntry value={formData.password} onChangeText={(t) => setFormData({...formData, password: t})} />
      </View>
      <View style={styles.inputGroup}>
        <Text style={styles.label}>Confirm Password *</Text>
        <TextInput style={styles.input} secureTextEntry value={formData.confirmPassword} onChangeText={(t) => setFormData({...formData, confirmPassword: t})} />
      </View>

      <Text style={styles.sectionTitle}>Verification Documents</Text>
      
      <View style={styles.uploadGroup}>
        <Text style={styles.label}>MBBS Degree *</Text>
        <Pressable style={styles.uploadButton} onPress={() => handlePickDocument(setMbbsFile, 'MBBS Degree')}>
          <Text style={styles.uploadButtonText}>{mbbsFile ? mbbsFile : 'Upload Document'}</Text>
        </Pressable>
      </View>

      <View style={styles.uploadGroup}>
        <Text style={styles.label}>Doctor's License *</Text>
        <Pressable style={styles.uploadButton} onPress={() => handlePickDocument(setLicenseFile, 'Doctor License')}>
          <Text style={styles.uploadButtonText}>{licenseFile ? licenseFile : 'Upload Document'}</Text>
        </Pressable>
      </View>

      <Pressable style={styles.checkboxContainer} onPress={() => setAcceptTerms(!acceptTerms)}>
        <View style={[styles.checkbox, acceptTerms && styles.checkboxSelected]} />
        <Text style={styles.checkboxLabel}>I accept the Terms and Conditions</Text>
      </Pressable>

      <Pressable style={styles.button} onPress={handleRegister} disabled={loading}>
        {loading ? <ActivityIndicator color="#fff" /> : <Text style={styles.buttonText}>Register & Continue</Text>}
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
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginTop: 10,
    marginBottom: 15,
    color: '#333',
  },
  inputGroup: {
    marginBottom: 15,
  },
  uploadGroup: {
    marginBottom: 20,
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
  uploadButton: {
    borderWidth: 1,
    borderColor: '#007bff',
    borderStyle: 'dashed',
    borderRadius: 8,
    padding: 15,
    alignItems: 'center',
    backgroundColor: '#f8f9fa',
  },
  uploadButtonText: {
    color: '#007bff',
    fontSize: 14,
    fontWeight: '500',
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
