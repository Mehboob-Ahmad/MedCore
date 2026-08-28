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
    // In a browser environment, we retrieve the token from localStorage
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('medichp_token');
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
    if (error.response && error.response.data && error.response.data.message) {
      // Backend returned a structured error message
      return Promise.reject(new Error(error.response.data.message));
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
    const response = await apiClient.get('/auth/me'); // Since auth/me returns the user profile
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
  }
};

export const DoctorService = {
  searchDoctors: async (searchTerm?: string, specialty?: string, gender?: string) => {
    const params = new URLSearchParams();
    if (searchTerm) params.append('searchTerm', searchTerm);
    if (specialty) params.append('specialty', specialty);
    if (gender) params.append('gender', gender);
    
    const response = await apiClient.get(`/doctors/search?${params.toString()}`);
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
  }
};

export const AdminService = {
  getStats: async () => {
    const response = await apiClient.get('/admin/stats');
    return response.data;
  }
};
