import axios from 'axios';

// Base URL configuration
// For Android emulator testing, localhost won't work. Use your machine's local IP (e.g., 192.168.1.X)
// Set this in apps/mobile/.env as EXPO_PUBLIC_API_URL=http://<YOUR_IP>:5188/api/v1
const API_URL = process.env.EXPO_PUBLIC_API_URL || 'https://medichp.onrender.com/api/v1';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const authService = {
  registerDoctor: async (data: any) => {
    const response = await api.post('/auth/register/doctor', data);
    return response.data;
  },
  registerPatient: async (data: any) => {
    const response = await api.post('/auth/register/patient', data);
    return response.data;
  },
  login: async (data: any) => {
    const response = await api.post('/auth/login', data);
    return response.data;
  },
  inviteAdmin: async (data: any) => {
    const response = await api.post('/auth/invite-admin', data);
    return response.data;
  },
  changePassword: async (data: any) => {
    const response = await api.post('/auth/change-password', data);
    return response.data;
  },
  updatePushToken: async (token: string) => {
    const response = await api.post('/auth/push-token', { pushToken: token });
    return response.data;
  },
};

export const doctorService = {
  getDashboard: async () => {
    const response = await api.get('/doctors/dashboard');
    return response.data;
  },
  getProfile: async () => {
    const response = await api.get('/doctors/profile');
    return response.data;
  },
  updateProfile: async (data: any) => {
    const response = await api.patch('/doctors/profile', data);
    return response.data;
  },
  configureAvailability: async (data: any) => {
    const response = await api.post('/doctors/availability', data);
    return response.data;
  },
};

export const patientService = {
  getDashboard: async () => {
    const response = await api.get('/patients/dashboard');
    return response.data;
  },
  searchDoctors: async (params: { searchTerm?: string; specialty?: string; gender?: string }) => {
    const response = await api.get('/doctors/search', { params });
    return response.data;
  },
  getProfile: async () => {
    const response = await api.get('/patients/profile');
    return response.data;
  },
  updateProfile: async (data: any) => {
    const response = await api.patch('/patients/profile', data);
    return response.data;
  },
};

export const appointmentService = {
  getAvailableSlots: async (doctorId: string, date: string) => {
    const response = await api.get(`/doctors/${doctorId}/slots`, { params: { date } });
    return response.data;
  },
  bookAppointment: async (data: { doctorId: string; scheduledDate: string; startTime: string; bookingNote: string }) => {
    const response = await api.post('/appointments', data);
    return response.data;
  },
  updateAppointmentStatus: async (appointmentId: string, status: string) => {
    const response = await api.patch(`/appointments/${appointmentId}/status`, { status });
    return response.data;
  },
};

export const chatService = {
  getConversations: async () => {
    const response = await api.get('/chat/conversations');
    return response.data;
  },
  getMessages: async (conversationId: string) => {
    const response = await api.get(`/chat/conversations/${conversationId}/messages`);
    return response.data;
  },
  sendMessage: async (conversationId: string, content: string) => {
    const response = await api.post(`/chat/conversations/${conversationId}/messages`, { content });
    return response.data;
  },
};

export const adminService = {
  getStats: async () => {
    const response = await api.get('/admin/stats');
    return response.data;
  },
};

export default api;
