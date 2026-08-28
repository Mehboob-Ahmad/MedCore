"use client";

import { useEffect, useState } from "react";
import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Calendar, Clock, MapPin, Search, Loader2 } from "lucide-react";
import Link from "next/link";
import { useAuth } from "@/contexts/AuthContext";
import { PatientService } from "@medichp/api-client";

export default function PatientDashboard() {
  const { user } = useAuth();
  const [stats, setStats] = useState<any>(null);
  const [profile, setProfile] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchDashboard = async () => {
      try {
        const [statsRes, profileRes] = await Promise.all([
          PatientService.getDashboardStats(),
          PatientService.getProfile()
        ]);
        
        if (statsRes.success) {
          setStats(statsRes.data);
        }
        if (profileRes.success) {
          setProfile(profileRes.data);
        }
      } catch (error) {
        console.error("Failed to load dashboard stats", error);
      } finally {
        setLoading(false);
      }
    };
    fetchDashboard();
  }, []);

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <Loader2 className="w-8 h-8 animate-spin text-gray-500" />
      </div>
    );
  }

  const isProfileIncomplete = profile && profile.profileCompletionPercentage < 100;

  return (
    <div className="space-y-6 pb-20 md:pb-0">
      {isProfileIncomplete && (
        <motion.div initial={{ opacity: 0, y: -10 }} animate={{ opacity: 1, y: 0 }} className="bg-amber-50 border border-amber-200 rounded-xl p-4 flex flex-col sm:flex-row items-center justify-between gap-4 shadow-sm">
          <div>
            <h3 className="text-amber-800 font-semibold text-sm sm:text-base">Your profile is incomplete</h3>
            <p className="text-amber-700 text-xs sm:text-sm">Complete your profile to book appointments and consult with doctors.</p>
          </div>
          <Link href="/patient/complete-profile">
            <Button variant="primary" className="bg-amber-500 hover:bg-amber-600 text-white whitespace-nowrap">
              Complete Profile
            </Button>
          </Link>
        </motion.div>
      )}

      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Hello, {user?.firstName || "Patient"}</h1>
          <p className="text-gray-500 dark:text-gray-400">Here's your health overview for today.</p>
        </div>
        <Link href="/search">
          <Button className="flex items-center gap-2">
            <Search className="w-4 h-4" />
            Find a Doctor
          </Button>
        </Link>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.1 }}>
          <Card className="bg-[var(--color-primary-600)] text-white border-none shadow-lg shadow-sky-900/10 h-full">
            <CardContent className="p-6">
              <h3 className="font-semibold text-sky-100 mb-1">Upcoming Appointments</h3>
              <div className="text-3xl font-bold mb-4">{stats?.upcomingAppointments || 0}</div>
              <Link href="/patient/appointments">
                <Button variant="outline" className="w-full text-[var(--color-primary-600)] border-white hover:bg-sky-50">View Schedule</Button>
              </Link>
            </CardContent>
          </Card>
        </motion.div>
        
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.2 }}>
          <Card className="h-full">
            <CardContent className="p-6">
              <h3 className="font-medium text-gray-500 dark:text-gray-400 mb-1">New Messages</h3>
              <div className="text-3xl font-bold text-gray-900 dark:text-white mb-2">{stats?.newMessages || 0}</div>
              <Link href="/patient/messages">
                <Button variant="outline" size="sm" className="w-full">Open Inbox</Button>
              </Link>
            </CardContent>
          </Card>
        </motion.div>
        
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.3 }}>
          <Card className="h-full">
            <CardContent className="p-6">
              <h3 className="font-medium text-gray-500 dark:text-gray-400 mb-1">Unread Notifications</h3>
              <div className="text-3xl font-bold text-gray-900 dark:text-white mb-2">{stats?.unreadNotifications || 0}</div>
              <Button variant="outline" size="sm" className="w-full">View Alerts</Button>
            </CardContent>
          </Card>
        </motion.div>
      </div>
    </div>
  );
}
