import { useState } from 'react';
import { StyleSheet, Text, View, TextInput, Pressable, ActivityIndicator, Alert, Platform } from 'react-native';
import { useRouter } from 'expo-router';
import { authService } from '../services/api';

export default function Login() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);

  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });

  const handleLogin = async () => {
    if (!formData.email || !formData.password) {
      Alert.alert('Error', 'Please enter your email and password.');
      return;
    }

    try {
      setLoading(true);
      const payload = {
        ...formData,
        deviceInfo: Platform.OS === 'android' ? 'Android Device' : Platform.OS === 'ios' ? 'iOS Device' : 'Web/Other',
      };

      const result = await authService.login(payload);
      // NOTE: In a real app we'd save the token from result to SecureStore here.
      const roles = result.data?.user?.roles || [];
      
      if (roles.includes('Doctor')) {
        router.replace('/(doctor)/dashboard');
      } else if (roles.includes('Patient')) {
        router.replace('/(patient)/dashboard');
      } else if (roles.includes('SystemAdmin')) {
        router.replace('/admin-dashboard');
      } else {
        router.replace('/');
      }
    } catch (error: any) {
      const msg = error.response?.data?.message || error.message || 'Login failed';
      Alert.alert('Login Failed', msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Welcome Back</Text>

      <View style={styles.inputGroup}>
        <Text style={styles.label}>Email</Text>
        <TextInput 
          style={styles.input} 
          keyboardType="email-address" 
          autoCapitalize="none" 
          value={formData.email} 
          onChangeText={(t) => setFormData({...formData, email: t})} 
        />
      </View>
      <View style={styles.inputGroup}>
        <Text style={styles.label}>Password</Text>
        <TextInput 
          style={styles.input} 
          secureTextEntry 
          value={formData.password} 
          onChangeText={(t) => setFormData({...formData, password: t})} 
        />
      </View>

      <Pressable style={styles.button} onPress={handleLogin} disabled={loading}>
        {loading ? <ActivityIndicator color="#fff" /> : <Text style={styles.buttonText}>Log In</Text>}
      </Pressable>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
    padding: 20,
    justifyContent: 'center',
  },
  title: {
    fontSize: 28,
    fontWeight: 'bold',
    marginBottom: 40,
    textAlign: 'center',
  },
  inputGroup: {
    marginBottom: 20,
  },
  label: {
    fontSize: 14,
    color: '#333',
    marginBottom: 8,
    fontWeight: '600',
  },
  input: {
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 8,
    padding: 15,
    fontSize: 16,
  },
  button: {
    backgroundColor: '#007bff',
    paddingVertical: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 20,
  },
  buttonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
  },
});
