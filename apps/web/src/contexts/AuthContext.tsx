"use client";

import React, { createContext, useContext, useState, useEffect } from "react";
import { AuthService } from "@medichp/api-client";
import { useRouter } from "next/navigation";

interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles?: string[];
  role?: string;
}

interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (credentials: any) => Promise<void>;
  logout: () => Promise<void>;
  forgotPassword: (data: { email: string }) => Promise<void>;
  resetPassword: (data: any) => Promise<void>;
  registerPatient: (data: any) => Promise<void>;
  registerDoctor: (data: any) => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    // Attempt to fetch profile on load if token exists
    const checkAuth = async () => {
      const token = localStorage.getItem("medichp_token");
      if (token) {
        try {
          const res = await AuthService.getProfile();
          if (res.success) {
            setUser(res.data);
          }
        } catch (error) {
          console.error("Session expired or invalid token");
          localStorage.removeItem("medichp_token");
          localStorage.removeItem("medichp_refresh_token");
        }
      }
      setLoading(false);
    };

    checkAuth();
  }, []);

  const login = async (credentials: any) => {
    const res = await AuthService.login(credentials);
    if (res.success && res.data) {
      localStorage.setItem("medichp_token", res.data.accessToken);
      localStorage.setItem("medichp_refresh_token", res.data.refreshToken);
      setUser(res.data.user);
      
      // Determine user role from the roles array (ASP.NET returns roles: ["RoleName"])
      const userRoles = res.data.user.roles || res.data.user.Roles || [];
      const primaryRole = userRoles.length > 0 ? userRoles[0] : res.data.user.role;
      
      // Redirect based on role
      if (primaryRole === "Doctor") {
        router.push("/doctor/dashboard");
      } else if (primaryRole === "Patient") {
        router.push("/patient/dashboard");
      } else if (primaryRole === "SystemAdmin" || primaryRole === "Admin") {
        router.push("/admin/dashboard");
      } else {
        router.push("/");
      }
    } else {
      throw new Error(res.message || "Login failed");
    }
  };

  const registerPatient = async (data: any) => {
    const res = await AuthService.registerPatient(data);
    if (!res.success) {
      throw new Error(res.message || "Registration failed");
    }
    // Often you want them to log in manually afterwards, or you can auto-login if the API returns a token
    if (res.data?.accessToken) {
      localStorage.setItem("medichp_token", res.data.accessToken);
      setUser(res.data.user);
      router.push("/patient/dashboard");
    } else {
      router.push("/login");
    }
  };

  const registerDoctor = async (data: any) => {
    const res = await AuthService.registerDoctor(data);
    if (!res.success) {
      throw new Error(res.message || "Registration failed");
    }
    if (res.data?.accessToken) {
      localStorage.setItem("medichp_token", res.data.accessToken);
      setUser(res.data.user);
      router.push("/doctor/dashboard");
    } else {
      router.push("/doctor/login");
    }
  };

  const logout = async () => {
    try {
      await AuthService.logout();
    } catch (e) {
      console.error("Logout API failed", e);
    }
    localStorage.removeItem("medichp_token");
    localStorage.removeItem("medichp_refresh_token");
    setUser(null);
    router.push("/");
  };

  const forgotPassword = async (data: { email: string }) => {
    await AuthService.forgotPassword(data);
  };

  const resetPassword = async (data: any) => {
    await AuthService.resetPassword(data);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, logout, forgotPassword, resetPassword, registerPatient, registerDoctor }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
