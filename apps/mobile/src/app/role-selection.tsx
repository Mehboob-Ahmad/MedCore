import { StyleSheet, Text, View, Pressable } from 'react-native';
import { Link } from 'expo-router';

export default function RoleSelection() {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Who are you?</Text>
      <Text style={styles.subtitle}>Please select your role to continue registration.</Text>
      
      <Link href="/register-patient" asChild>
        <Pressable style={styles.button}>
          <Text style={styles.buttonText}>I am a Patient</Text>
        </Pressable>
      </Link>

      <Link href="/register-doctor" asChild>
        <Pressable style={StyleSheet.flatten([styles.button, styles.doctorButton])}>
          <Text style={styles.doctorButtonText}>I am a Doctor</Text>
        </Pressable>
      </Link>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 20,
  },
  title: {
    fontSize: 28,
    fontWeight: 'bold',
    marginBottom: 10,
  },
  subtitle: {
    fontSize: 16,
    color: '#666',
    marginBottom: 40,
    textAlign: 'center',
  },
  button: {
    backgroundColor: '#007bff',
    paddingVertical: 15,
    paddingHorizontal: 30,
    borderRadius: 8,
    width: '100%',
    alignItems: 'center',
    marginBottom: 15,
  },
  buttonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: 'bold',
  },
  doctorButton: {
    backgroundColor: '#fff',
    borderWidth: 2,
    borderColor: '#007bff',
  },
  doctorButtonText: {
    color: '#007bff',
    fontSize: 18,
    fontWeight: 'bold',
  },
});
