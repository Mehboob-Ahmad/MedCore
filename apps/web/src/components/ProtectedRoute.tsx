"use client";

import React, { useEffect, useState } from "react";
import { useAuth } from "@/contexts/AuthContext";
import { useRouter } from "next/navigation";
import { Loader2 } from "lucide-react";

interface ProtectedRouteProps {
  children: React.ReactNode;
  allowedRoles?: string[];
}

export function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
  const { isAuthenticated, loading, role } = useAuth();
  const router = useRouter();
  const [isAuthorized, setIsAuthorized] = useState(false);

  useEffect(() => {
    if (!loading) {
      if (!isAuthenticated) {
        router.replace("/login");
        return;
      }

      if (allowedRoles && allowedRoles.length > 0) {
        const userRole = role || "";
        // Support role variations (e.g., SystemAdmin vs Admin)
        const hasRole = allowedRoles.some(r => {
          if (r === "Admin" && userRole === "SystemAdmin") return true;
          if (r === "SystemAdmin" && userRole === "Admin") return true;
          return r === userRole;
        });

        if (!hasRole) {
          // Redirect to their appropriate dashboard if they try to access wrong role route
          if (userRole === "Patient") router.replace("/patient/dashboard");
          else if (userRole === "Doctor") router.replace("/doctor/dashboard");
          else if (userRole === "Admin" || userRole === "SystemAdmin") router.replace("/admin/dashboard");
          else router.replace("/");
          return;
        }
      }

      setIsAuthorized(true);
    }
  }, [loading, isAuthenticated, role, allowedRoles, router]);

  if (loading || !isAuthorized) {
    return (
      <div className="flex-1 flex items-center justify-center bg-slate-50 dark:bg-slate-900 h-full min-h-[50vh]">
        <Loader2 className="w-10 h-10 animate-spin text-[var(--color-primary-600)]" />
      </div>
    );
  }

  return <>{children}</>;
}
