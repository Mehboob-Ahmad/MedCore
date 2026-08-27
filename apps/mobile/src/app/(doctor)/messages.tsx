import { useState, useEffect } from 'react';
import { StyleSheet, Text, View, ScrollView, Pressable, ActivityIndicator } from 'react-native';
import { useRouter } from 'expo-router';
import { chatService } from '../../services/api';

export default function DoctorMessages() {
  const router = useRouter();
  const [loading, setLoading] = useState(true);
  const [conversations, setConversations] = useState<any[]>([]);

  useEffect(() => {
    loadConversations();
  }, []);

  const loadConversations = async () => {
    try {
      setLoading(true);
      const response = await chatService.getConversations();
      if (response.success) {
        setConversations(response.data || []);
      }
    } catch (error: any) {
      // Mock data for development
      setConversations([
        {
          id: 'mock-conv-1',
          otherParticipantName: 'John Doe',
          unreadCount: 1,
          lastMessage: {
            content: 'Thank you doctor, I will book it now.',
            sentAt: new Date().toISOString(),
          }
        },
        {
          id: 'mock-conv-3',
          otherParticipantName: 'Emily Clark',
          unreadCount: 0,
          lastMessage: {
            content: 'Is it okay if I take this medication with food?',
            sentAt: new Date(Date.now() - 3600000).toISOString(),
          }
        }
      ]);
    } finally {
      setLoading(false);
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
      {conversations.length === 0 ? (
        <View style={styles.emptyContainer}>
          <Text style={styles.emptyText}>You have no messages yet.</Text>
        </View>
      ) : (
        conversations.map((conv) => {
          const dateStr = conv.lastMessage?.sentAt 
            ? new Date(conv.lastMessage.sentAt).toLocaleDateString([], { month: 'short', day: 'numeric' })
            : '';
            
          return (
            <Pressable 
              key={conv.id} 
              style={styles.chatCard}
              onPress={() => router.push(`/chat/${conv.id}`)}
            >
              <View style={styles.avatarPlaceholder}>
                <Text style={styles.avatarText}>{conv.otherParticipantName.substring(0, 1).toUpperCase()}</Text>
              </View>
              
              <View style={styles.chatInfo}>
                <View style={styles.chatHeader}>
                  <Text style={styles.chatName}>{conv.otherParticipantName}</Text>
                  <Text style={styles.chatDate}>{dateStr}</Text>
                </View>
                
                <View style={styles.messageRow}>
                  <Text style={[styles.lastMessage, conv.unreadCount > 0 && styles.lastMessageUnread]} numberOfLines={1}>
                    {conv.lastMessage?.content || 'No messages yet'}
                  </Text>
                  
                  {conv.unreadCount > 0 && (
                    <View style={styles.unreadBadge}>
                      <Text style={styles.unreadText}>{conv.unreadCount}</Text>
                    </View>
                  )}
                </View>
              </View>
            </Pressable>
          );
        })
      )}
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
    padding: 15,
  },
  emptyContainer: {
    padding: 40,
    alignItems: 'center',
  },
  emptyText: {
    fontSize: 16,
    color: '#888',
  },
  chatCard: {
    flexDirection: 'row',
    paddingVertical: 15,
    borderBottomWidth: 1,
    borderBottomColor: '#f0f0f0',
    alignItems: 'center',
  },
  avatarPlaceholder: {
    width: 50,
    height: 50,
    borderRadius: 25,
    backgroundColor: '#e6f7ff',
    justifyContent: 'center',
    alignItems: 'center',
    marginRight: 15,
  },
  avatarText: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#0050b3',
  },
  chatInfo: {
    flex: 1,
  },
  chatHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 5,
  },
  chatName: {
    fontSize: 16,
    fontWeight: 'bold',
    color: '#222',
  },
  chatDate: {
    fontSize: 12,
    color: '#888',
  },
  messageRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  lastMessage: {
    fontSize: 14,
    color: '#666',
    flex: 1,
    paddingRight: 10,
  },
  lastMessageUnread: {
    fontWeight: 'bold',
    color: '#333',
  },
  unreadBadge: {
    backgroundColor: '#dc3545',
    borderRadius: 10,
    paddingHorizontal: 6,
    paddingVertical: 2,
    minWidth: 20,
    alignItems: 'center',
  },
  unreadText: {
    color: '#fff',
    fontSize: 10,
    fontWeight: 'bold',
  },
});
