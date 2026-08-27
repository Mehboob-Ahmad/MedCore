import { useState, useEffect } from 'react';
import { StyleSheet, Text, View, ScrollView, TextInput, Pressable, ActivityIndicator, Alert, Image } from 'react-native';
import { useRouter } from 'expo-router';
import { patientService } from '../../services/api';

export default function DoctorSearch() {
  const router = useRouter();
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState<any[]>([]);
  const [hasSearched, setHasSearched] = useState(false);

  // Load some defaults on mount
  useEffect(() => {
    handleSearch('');
  }, []);

  const handleSearch = async (term: string) => {
    try {
      setLoading(true);
      setHasSearched(true);
      const response = await patientService.searchDoctors({ searchTerm: term });
      if (response.success) {
        setResults(response.data || []);
      }
    } catch (error: any) {
      // Mock data for development
      setResults([
        {
          doctorId: '1',
          firstName: 'Sarah',
          lastName: 'Jenkins',
          experienceYears: 12,
          bio: 'Experienced cardiologist specializing in heart failure.',
          consultationFee: 150,
          specializations: ['Cardiology'],
          rating: 4.8,
          reviewCount: 124
        },
        {
          doctorId: '2',
          firstName: 'Michael',
          lastName: 'Chen',
          experienceYears: 8,
          bio: 'General practitioner focused on family medicine and pediatrics.',
          consultationFee: 90,
          specializations: ['General Practice', 'Pediatrics'],
          rating: 4.9,
          reviewCount: 89
        }
      ].filter(d => 
        (d.firstName + ' ' + d.lastName).toLowerCase().includes(term.toLowerCase()) ||
        d.specializations.some(s => s.toLowerCase().includes(term.toLowerCase()))
      ));
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <View style={styles.searchHeader}>
        <TextInput
          style={styles.searchInput}
          placeholder="Search by name, specialty, or condition..."
          value={searchTerm}
          onChangeText={setSearchTerm}
          onSubmitEditing={() => handleSearch(searchTerm)}
          returnKeyType="search"
        />
        <Pressable style={styles.searchButton} onPress={() => handleSearch(searchTerm)}>
          <Text style={styles.searchButtonText}>Search</Text>
        </Pressable>
      </View>

      {loading ? (
        <View style={styles.centerContainer}>
          <ActivityIndicator size="large" color="#28a745" />
        </View>
      ) : (
        <ScrollView contentContainerStyle={styles.resultsContainer}>
          {hasSearched && results.length === 0 ? (
            <View style={styles.centerContainer}>
              <Text style={styles.noResultsText}>No doctors found matching "{searchTerm}"</Text>
            </View>
          ) : (
            results.map((doctor) => (
              <View key={doctor.doctorId} style={styles.doctorCard}>
                <View style={styles.doctorHeader}>
                  <View style={styles.avatarPlaceholder}>
                    <Text style={styles.avatarText}>{doctor.firstName[0]}{doctor.lastName[0]}</Text>
                  </View>
                  <View style={styles.doctorInfo}>
                    <Text style={styles.doctorName}>Dr. {doctor.firstName} {doctor.lastName}</Text>
                    <Text style={styles.doctorSpecialty}>
                      {doctor.specializations?.join(', ')} • {doctor.experienceYears} Yrs Exp
                    </Text>
                    <View style={styles.ratingRow}>
                      <Text style={styles.ratingText}>⭐ {doctor.rating} ({doctor.reviewCount} reviews)</Text>
                    </View>
                  </View>
                </View>
                
                <Text style={styles.doctorBio} numberOfLines={2}>{doctor.bio}</Text>
                
                <View style={styles.cardFooter}>
                  <Text style={styles.feeText}>${doctor.consultationFee} / Visit</Text>
                  <Pressable style={styles.bookButton} onPress={() => router.push(`/(patient)/book/${doctor.doctorId}`)}>
                    <Text style={styles.bookButtonText}>Book Appointment</Text>
                  </Pressable>
                </View>
              </View>
            ))
          )}
        </ScrollView>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f4f6f8',
  },
  searchHeader: {
    flexDirection: 'row',
    padding: 15,
    backgroundColor: '#fff',
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
  },
  searchInput: {
    flex: 1,
    backgroundColor: '#f0f0f0',
    paddingHorizontal: 15,
    paddingVertical: 10,
    borderRadius: 8,
    fontSize: 16,
    marginRight: 10,
  },
  searchButton: {
    backgroundColor: '#28a745',
    paddingHorizontal: 20,
    justifyContent: 'center',
    borderRadius: 8,
  },
  searchButtonText: {
    color: '#fff',
    fontWeight: 'bold',
  },
  centerContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 40,
  },
  resultsContainer: {
    padding: 15,
    paddingBottom: 30,
  },
  noResultsText: {
    fontSize: 16,
    color: '#666',
    textAlign: 'center',
  },
  doctorCard: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 15,
    marginBottom: 15,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 5,
    elevation: 3,
  },
  doctorHeader: {
    flexDirection: 'row',
    marginBottom: 12,
  },
  avatarPlaceholder: {
    width: 60,
    height: 60,
    borderRadius: 30,
    backgroundColor: '#e6f4ea',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 15,
  },
  avatarText: {
    color: '#1e7e34',
    fontSize: 20,
    fontWeight: 'bold',
  },
  doctorInfo: {
    flex: 1,
    justifyContent: 'center',
  },
  doctorName: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#222',
    marginBottom: 2,
  },
  doctorSpecialty: {
    fontSize: 14,
    color: '#666',
    marginBottom: 4,
  },
  ratingRow: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  ratingText: {
    fontSize: 13,
    color: '#ffc107',
    fontWeight: '600',
  },
  doctorBio: {
    fontSize: 14,
    color: '#555',
    lineHeight: 20,
    marginBottom: 15,
  },
  cardFooter: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderTopWidth: 1,
    borderTopColor: '#f0f0f0',
    paddingTop: 15,
  },
  feeText: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#333',
  },
  bookButton: {
    backgroundColor: '#28a745',
    paddingVertical: 10,
    paddingHorizontal: 20,
    borderRadius: 8,
  },
  bookButtonText: {
    color: '#fff',
    fontWeight: 'bold',
    fontSize: 14,
  }
});
