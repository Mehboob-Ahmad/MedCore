import { StyleSheet, Text, View, Pressable } from 'react-native';
import { Link } from 'expo-router';

export default function Home() {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>MediCore Mobile</Text>
      
      <Link href="/role-selection" asChild>
        <Pressable style={styles.button}>
          <Text style={styles.buttonText}>Register / Sign Up</Text>
        </Pressable>
      </Link>

      <Link href="/login" asChild>
        <Pressable style={StyleSheet.flatten([styles.button, styles.outlineButton])}>
          <Text style={styles.outlineButtonText}>Log In</Text>
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
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 40,
  },
  button: {
    backgroundColor: '#007bff',
    paddingVertical: 15,
    paddingHorizontal: 30,
    borderRadius: 8,
    width: '100%',
    alignItems: 'center',
  },
  buttonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: 'bold',
  },
  secondaryButton: {
    backgroundColor: '#28a745',
    marginTop: 15,
  },
  secondaryButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: 'bold',
  },
  outlineButton: {
    backgroundColor: 'transparent',
    borderWidth: 2,
    borderColor: '#007bff',
    marginTop: 30,
  },
  outlineButtonText: {
    color: '#007bff',
    fontSize: 16,
    fontWeight: 'bold',
  },
});
