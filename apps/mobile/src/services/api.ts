import axios from 'axios';

const API_URL = process.env.EXPO_PUBLIC_API_URL || 'https://medihc-api.onrender.com';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const authService = {
  registerDoctor: async (data: any) => {
    const response = await api.post('/api/v1/auth/register/doctor', data);
    return response.data;
  },
  registerPatient: async (data: any) => {
    const response = await api.post('/api/v1/auth/register/patient', data);
    return response.data;
  },
  login: async (data: any) => {
    const response = await api.post('/api/v1/auth/login', data);
    return response.data;
  },
  inviteAdmin: async (data: any) => {
    const response = await api.post('/api/v1/auth/invite-admin', data);
    return response.data;
  },
  changePassword: async (data: any) => {
    const response = await api.post('/api/v1/auth/change-password', data);
    return response.data;
  },
  updatePushToken: async (token: string) => {
    const response = await api.post('/api/v1/auth/push-token', { pushToken: token });
    return response.data;
  },
};

export const doctorService = {
  getDashboard: async () => {
    const response = await api.get('/api/v1/doctors/dashboard');
    return response.data;
  },
  getProfile: async () => {
    const response = await api.get('/api/v1/doctors/profile');
    return response.data;
  },
  updateProfile: async (data: any) => {
    const response = await api.patch('/api/v1/doctors/profile', data);
    return response.data;
  },
  configureAvailability: async (data: any) => {
    const response = await api.post('/api/v1/doctors/availability', data);
    return response.data;
  },
};

export const patientService = {
  getDashboard: async () => {
    const response = await api.get('/api/v1/patients/dashboard');
    return response.data;
  },
  searchDoctors: async (params: { searchTerm?: string; specialty?: string; gender?: string }) => {
    const response = await api.get('/api/v1/doctors/search', { params });
    return response.data;
  },
  getProfile: async () => {
    const response = await api.get('/api/v1/patients/profile');
    return response.data;
  },
  updateProfile: async (data: any) => {
    const response = await api.patch('/api/v1/patients/profile', data);
    return response.data;
  },
};

export const appointmentService = {
  getAvailableSlots: async (doctorId: string, date: string) => {
    const response = await api.get(`/api/v1/doctors/${doctorId}/slots`, { params: { date } });
    return response.data;
  },
  bookAppointment: async (data: { doctorId: string; scheduledDate: string; startTime: string; bookingNote: string }) => {
    const response = await api.post('/api/v1/appointments', data);
    return response.data;
  },
  updateAppointmentStatus: async (appointmentId: string, status: string) => {
    const response = await api.patch(`/api/v1/appointments/${appointmentId}/status`, { status });
    return response.data;
  },
};

export const chatService = {
  getConversations: async () => {
    const response = await api.get('/api/v1/chat/conversations');
    return response.data;
  },
  getMessages: async (conversationId: string) => {
    const response = await api.get(`/api/v1/chat/conversations/${conversationId}/messages`);
    return response.data;
  },
  sendMessage: async (conversationId: string, content: string) => {
    const response = await api.post(`/api/v1/chat/conversations/${conversationId}/messages`, { content });
    return response.data;
  },
};

export const adminService = {
  getStats: async () => {
    const response = await api.get('/api/v1/admin/stats');
    return response.data;
  },
};

export default api;
