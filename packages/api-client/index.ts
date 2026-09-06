import axios from 'axios';

// Base URL configuration - points to the .NET API
// Forcing Render URL directly to bypass Vercel environment variable caching issues
const API_BASE_URL = 'https://medichp.onrender.com/api/v1';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor to attach the JWT token to every request
apiClient.interceptors.request.use(
  (config) => {
    // In a browser environment, we retrieve the token from localStorage or sessionStorage
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('medichp_token') || sessionStorage.getItem('medichp_token');
      if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor to handle responses and standardise error messages
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      if (error.response.status === 401) {
        if (typeof window !== 'undefined') {
          localStorage.removeItem('medichp_token');
          localStorage.removeItem('medichp_refresh_token');
          window.location.href = '/login';
        }
      }

      if (error.response.status === 403) {
        // Handle 403 Forbidden without logging the user out.
        // We throw an explicit error message. A global error boundary or component can catch it,
        // or we can redirect to a generic unauthorized page.
        if (typeof window !== 'undefined' && window.location.pathname.includes('/admin')) {
             // If they are not an admin but trying to access admin, redirect out
             window.location.href = '/';
        }
        return Promise.reject(new Error("Access Denied: You do not have permission to view this resource."));
      }
      
      const data = error.response.data;
      if (data && data.message) {
        // Backend returned a structured error message
        let errorMessage = data.message;
        if (data.errors && Array.isArray(data.errors) && data.errors.length > 0) {
           errorMessage += ": " + data.errors.map((e: any) => e.errorMessage || e).join(", ");
        }
        return Promise.reject(new Error(errorMessage));
      }
      
      if (error.response.status >= 500) {
        return Promise.reject(new Error("A server error occurred. Please try again later."));
      }
    }
    return Promise.reject(error);
  }
);

// Auth Service Endpoints
export const AuthService = {
  login: async (data: any) => {
    const response = await apiClient.post('/auth/login', data);
    return response.data;
  },
  registerPatient: async (data: any) => {
    const response = await apiClient.post('/auth/register/patient', data);
    return response.data;
  },
  registerDoctor: async (data: any) => {
    const response = await apiClient.post('/auth/register/doctor', data);
    return response.data;
  },
  getProfile: async () => {
    const response = await apiClient.get('/auth/me');
    return response.data;
  },
  inviteAdmin: async (data: { email: string; firstName: string; lastName: string; phoneNumber: string }) => {
    const response = await apiClient.post('/auth/invite-admin', data);
    return response.data;
  },
  changePassword: async (data: any) => {
    const response = await apiClient.post('/auth/change-password', data);
    return response.data;
  },
  forgotPassword: async (data: { email: string }) => {
    const response = await apiClient.post('/auth/forgot-password', data);
    return response.data;
  },
  resetPassword: async (data: any) => {
    const response = await apiClient.post('/auth/reset-password', data);
    return response.data;
  },
  logout: async () => {
    if (typeof window !== 'undefined') {
      const refreshToken = localStorage.getItem('medichp_refresh_token');
      if (refreshToken) {
        try {
          await apiClient.post('/auth/logout', { refreshToken });
        } catch (e) {
          console.error("Logout API failed", e);
        }
      }
      localStorage.removeItem('medichp_token');
      localStorage.removeItem('medichp_refresh_token');
    }
  },
  uploadFile: async (file: File, purpose: string = "General") => {
    const formData = new FormData();
    formData.append("file", file);
    formData.append("purpose", purpose);
    const response = await apiClient.post('/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  }
};

export const PublicService = {
  submitDemoRequest: async (data: any) => {
    const response = await apiClient.post('/public/demo-requests', data);
    return response.data;
  }
};

// Chat Service Endpoints
export const ChatService = {
  getConversations: async () => {
    const response = await apiClient.get('/chat/conversations');
    return response.data;
  },
  getMessages: async (conversationId: string) => {
    const response = await apiClient.get(`/chat/conversations/${conversationId}/messages`);
    return response.data;
  },
  sendMessage: async (conversationId: string, data: { content?: string; messageType: string; attachmentId?: string }) => {
    const response = await apiClient.post(`/chat/conversations/${conversationId}/messages`, data);
    return response.data;
  },
  startConversation: async (targetUserId: string) => {
    const response = await apiClient.post(`/chat/conversations`, { targetUserId });
    return response.data;
  },
  markAsRead: async (conversationId: string) => {
    const response = await apiClient.post(`/chat/conversations/${conversationId}/read`);
    return response.data;
  },
  uploadChatMedia: async (file: File) => {
    const formData = new FormData();
    formData.append("file", file);
    const response = await apiClient.post('/chat/attachments', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  }
};

// Patient Service Endpoints
export const PatientService = {
  getAppointments: async (filter?: string, status?: string) => {
    const params = new URLSearchParams();
    if (filter) params.append('filter', filter);
    if (status) params.append('status', status);
    const response = await apiClient.get(`/appointments/patient?${params.toString()}`);
    return response.data;
  },
  getProfile: async () => {
    const response = await apiClient.get('/patients/profile');
    return response.data;
  },
  updateProfile: async (data: any) => {
    const response = await apiClient.patch('/patients/profile', data);
    return response.data;
  },
  // Future implementation for stats/messages if backend adds them explicitly
  getDashboardStats: async () => {
    // Placeholder until backend implements /patients/dashboard-stats
    return { success: true, data: { upcomingAppointments: 0, newMessages: 0, unreadNotifications: 0 } };
  },
  getMessages: async () => {
    // Placeholder until backend implements /chat/threads
    return { success: true, data: [] };
  },
  getReports: async () => {
    const response = await apiClient.get('/patients/reports');
    return response.data;
  },
  addAllergy: async (data: any) => {
    const response = await apiClient.post('/patients/allergies', data);
    return response.data;
  },
  updateAllergy: async (id: string, data: any) => {
    const response = await apiClient.put(`/patients/allergies/${id}`, data);
    return response.data;
  },
  deleteAllergy: async (id: string) => {
    const response = await apiClient.delete(`/patients/allergies/${id}`);
    return response.data;
  }
};

export const SystemService = {
  getCities: async () => {
    const response = await apiClient.get('/system/cities');
    return response.data;
  },
};

export const AiService = {
  getHistory: async () => {
    const response = await apiClient.get('/ai/history');
    return response.data;
  },
  ask: async (prompt: string) => {
    const response = await apiClient.post('/ai/ask', { prompt });
    return response.data;
  }
};

export const DoctorService = {
  searchDoctors: async (searchTerm?: string, specialty?: string, gender?: string, cityIds?: string[]) => {
    const params = new URLSearchParams();
    if (searchTerm) params.append('searchTerm', searchTerm);
    if (specialty) params.append('specialty', specialty);
    if (gender) params.append('gender', gender);
    if (cityIds && cityIds.length > 0) {
      cityIds.forEach(id => params.append('cityIds', id));
    }
    
    const response = await apiClient.get(`/doctors/search?${params.toString()}`);
    return response.data;
  },
  getProfile: async () => {
    const response = await apiClient.get('/doctors/profile');
    return response.data;
  },
  updateProfile: async (data: any) => {
    const response = await apiClient.patch('/doctors/profile', data);
    return response.data;
  },
  completeProfile: async (data: any) => {
    const response = await apiClient.put('/doctors/profile/complete', data);
    return response.data;
  },
  getAppointments: async (filter?: string, status?: string, dateFrom?: string) => {
    const params = new URLSearchParams();
    if (filter) params.append('filter', filter);
    if (status) params.append('status', status);
    if (dateFrom) params.append('dateFrom', dateFrom);
    const response = await apiClient.get(`/appointments/doctor?${params.toString()}`);
    return response.data;
  },
  getDashboardStats: async () => {
    // Placeholder until backend implements /doctors/dashboard-stats
    return { success: true, data: { totalAppointments: 0, pendingRequests: 0, newMessages: 0 } };
  },
  getMessages: async () => {
    // Placeholder until backend implements /chat/threads
    return { success: true, data: [] };
  },
  getPatientClinicalSummary: async (patientId: string) => {
    const response = await apiClient.get(`/patients/${patientId}/clinical-summary`);
    return response.data;
  },
  addPatient: async (data: any) => {
    const response = await apiClient.post('/doctors/patients', data);
    return response.data;
  },
  getDoctor: async (id: string) => {
    const response = await apiClient.get(`/doctors/${id}`);
    return response.data;
  },
  getAvailableSlots: async (id: string, date: string) => {
    const response = await apiClient.get(`/doctors/${id}/slots?date=${date}`);
    return response.data;
  },
};

export const AdminService = {
  getStats: async () => {
    const response = await apiClient.get('/admin/stats');
    return response.data;
  },
  getSpecialties: async () => {
    const response = await apiClient.get('/system/specialties');
    return response.data;
  },
  getUsers: async () => {
    const response = await apiClient.get('/admin/users');
    return response.data;
  },
  toggleUserStatus: async (id: string, isActive: boolean, reason?: string | null) => {
    const response = await apiClient.post(`/admin/users/${id}/toggle-status`, { isActive, reason });
    return response.data;
  },
  getDemoRequests: async () => {
    const response = await apiClient.get('/admin/demo-requests');
    return response.data;
  },
  updateDemoRequestStatus: async (id: string, data: { status: number; notes?: string }) => {
    const response = await apiClient.put(`/admin/demo-requests/${id}/status`, data);
    return response.data;
  },
  createDoctorFromDemo: async (id: string) => {
    const response = await apiClient.post(`/admin/demo-requests/${id}/create-doctor`);
    return response.data;
  }
};

