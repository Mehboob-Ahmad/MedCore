import { useState, useEffect } from 'react';
import { StyleSheet, Text, View, ScrollView, TextInput, Pressable, ActivityIndicator, Alert } from 'react-native';
import { doctorService } from '../../services/api';

export default function DoctorProfile() {
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [formData, setFormData] = useState({
    bio: '',
    consultationFee: '',
    experienceYears: '',
  });

  useEffect(() => {
    loadProfile();
  }, []);

  const loadProfile = async () => {
    try {
      setLoading(true);
      const response = await doctorService.getProfile();
      if (response.success && response.data) {
        setFormData({
          bio: response.data.bio || '',
          consultationFee: response.data.consultationFee?.toString() || '0',
          experienceYears: response.data.experienceYears?.toString() || '0',
        });
      }
    } catch (error: any) {
      // Fallback for dev without auth
      setFormData({
        bio: 'I am a highly experienced doctor committed to patient care.',
        consultationFee: '150',
        experienceYears: '10',
      });
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async () => {
    try {
      setSaving(true);
      const payload = {
        bio: formData.bio,
        consultationFee: parseFloat(formData.consultationFee) || 0,
        experienceYears: parseInt(formData.experienceYears, 10) || 0,
      };

      await doctorService.updateProfile(payload);
      Alert.alert('Success', 'Profile updated successfully!');
    } catch (error: any) {
      const msg = error.response?.data?.message || error.message || 'Failed to update profile';
      Alert.alert('Error', msg);
    } finally {
      setSaving(false);
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
      <Text style={styles.title}>My Profile</Text>
      <Text style={styles.subtitle}>Update your professional details. These are visible to patients when they search for you.</Text>

      <View style={styles.inputGroup}>
        <Text style={styles.label}>Professional Bio</Text>
        <TextInput
          style={[styles.input, styles.textArea]}
          multiline
          numberOfLines={4}
          placeholder="Tell patients about your background, expertise, and approach to care..."
          value={formData.bio}
          onChangeText={(val) => setFormData({ ...formData, bio: val })}
        />
      </View>

      <View style={styles.row}>
        <View style={[styles.inputGroup, { flex: 1, marginRight: 10 }]}>
          <Text style={styles.label}>Consultation Fee ($)</Text>
          <TextInput
            style={styles.input}
            keyboardType="numeric"
            placeholder="150"
            value={formData.consultationFee}
            onChangeText={(val) => setFormData({ ...formData, consultationFee: val })}
          />
        </View>

        <View style={[styles.inputGroup, { flex: 1, marginLeft: 10 }]}>
          <Text style={styles.label}>Experience (Years)</Text>
          <TextInput
            style={styles.input}
            keyboardType="numeric"
            placeholder="10"
            value={formData.experienceYears}
            onChangeText={(val) => setFormData({ ...formData, experienceYears: val })}
          />
        </View>
      </View>

      <Pressable style={styles.saveButton} onPress={handleSave} disabled={saving}>
        {saving ? <ActivityIndicator color="#fff" /> : <Text style={styles.saveButtonText}>Save Profile</Text>}
      </Pressable>
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
    backgroundColor: '#fff',
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
    marginBottom: 30,
    lineHeight: 20,
  },
  inputGroup: {
    marginBottom: 20,
  },
  row: {
    flexDirection: 'row',
    justifyContent: 'space-between',
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
    padding: 12,
    fontSize: 16,
    backgroundColor: '#fafafa',
  },
  textArea: {
    height: 120,
    textAlignVertical: 'top',
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
