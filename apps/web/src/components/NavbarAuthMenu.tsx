"use client";

import Link from "next/link";
import { useAuth } from "@/contexts/AuthContext";
import { LogOut, User } from "lucide-react";
import { useRouter } from "next/navigation";

export function NavbarAuthMenu() {
  const { user, loading, logout } = useAuth();
  const router = useRouter();

  const handleLogout = async () => {
    try {
      await logout();
      router.push("/login");
    } catch (e) {
      console.error(e);
    }
  };

  if (loading) {
    return <div className="w-24 h-8 animate-pulse bg-slate-200 dark:bg-slate-700 rounded-md"></div>;
  }

  if (user) {
    const userRoles = user.roles || (user as any).Roles || [];
    const primaryRole = userRoles.length > 0 ? userRoles[0] : user.role;
    
    let dashboardLink = "/";
    if (primaryRole === "Doctor") dashboardLink = "/doctor/dashboard";
    else if (primaryRole === "Patient") dashboardLink = "/patient/dashboard";
    else if (primaryRole === "SystemAdmin" || primaryRole === "Admin") dashboardLink = "/admin/dashboard";

    return (
      <div className="flex items-center gap-4">
        <Link href={dashboardLink} className="flex items-center gap-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-[var(--color-primary-600)] transition-colors">
          <User className="w-4 h-4" />
          <span>{user.firstName}</span>
        </Link>
        <button
          onClick={handleLogout}
          className="flex items-center gap-2 text-sm font-medium text-red-600 hover:text-red-700 transition-colors"
        >
          <LogOut className="w-4 h-4" />
          Log out
        </button>
      </div>
    );
  }

  return (
    <div className="flex items-center gap-4">
      <Link href="/login" className="text-sm font-medium hover:text-[var(--color-primary-600)]">
        Log in
      </Link>
      <Link
        href="/register"
        className="text-sm font-medium bg-[var(--color-primary-600)] text-white px-4 py-2 rounded-lg hover:opacity-90 transition-opacity"
      >
        Sign up
      </Link>
    </div>
  );
}
