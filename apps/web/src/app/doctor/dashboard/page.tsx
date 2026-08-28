"use client";

import { useEffect, useState } from "react";
import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Users, Clock, Calendar, CheckCircle, Loader2 } from "lucide-react";
import Link from "next/link";
import { useAuth } from "@/contexts/AuthContext";
import { DoctorService } from "@medichp/api-client";

export default function DoctorDashboard() {
  const { user } = useAuth();
  const [stats, setStats] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchDashboard = async () => {
      try {
        const res = await DoctorService.getDashboardStats();
        if (res.success) {
          setStats(res.data);
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

  return (
    <div className="space-y-6 pb-20 md:pb-0">
      <div>
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Dr. {user?.lastName || "Smith"}'s Dashboard</h1>
        <p className="text-gray-500 dark:text-gray-400">Here's your practice overview for today.</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.1 }}>
          <Card className="bg-[var(--color-primary-600)] text-white border-none shadow-lg shadow-sky-900/10 h-full">
            <CardContent className="p-6">
              <h3 className="font-medium text-sky-100 mb-1">Total Appointments</h3>
              <div className="text-3xl font-bold mb-2">{stats?.totalAppointments || 0}</div>
              <p className="text-sm text-sky-200">Today</p>
            </CardContent>
          </Card>
        </motion.div>
        
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.2 }}>
          <Card className="h-full">
            <CardContent className="p-6">
              <h3 className="font-medium text-gray-500 dark:text-gray-400 mb-1">Pending Requests</h3>
              <div className="text-3xl font-bold text-gray-900 dark:text-white mb-2">{stats?.pendingRequests || 0}</div>
              <p className="text-sm text-amber-500">Requires attention</p>
            </CardContent>
          </Card>
        </motion.div>

        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.3 }}>
          <Card className="h-full">
            <CardContent className="p-6">
              <h3 className="font-medium text-gray-500 dark:text-gray-400 mb-1">New Messages</h3>
              <div className="text-3xl font-bold text-gray-900 dark:text-white mb-2">{stats?.newMessages || 0}</div>
              <p className="text-sm text-[var(--color-secondary-500)]">From patients</p>
            </CardContent>
          </Card>
        </motion.div>
      </div>

      <div className="flex justify-between items-end mt-8 mb-4">
        <h2 className="text-xl font-bold text-gray-900 dark:text-white">Next Patient</h2>
        <Link href="/doctor/schedule">
          <Button variant="ghost" size="sm" className="text-[var(--color-primary-600)]">View Schedule &rarr;</Button>
        </Link>
      </div>
      
      {!stats?.nextAppointment ? (
        <Card className="bg-slate-50 dark:bg-slate-900/50 border-dashed border-2">
          <CardContent className="p-10 text-center text-gray-500">
            No upcoming appointments for today.
          </CardContent>
        </Card>
      ) : (
        <Card>
          <CardContent className="p-6 flex flex-col md:flex-row md:items-center justify-between gap-4">
            <div>
              <div className="flex items-center gap-2 mb-2">
                <span className="px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                  {stats.nextAppointment.status || "Confirmed"}
                </span>
                <span className="flex items-center gap-1 text-xs text-gray-500 font-medium">
                  <Clock className="w-3 h-3" /> {new Date(stats.nextAppointment.dateTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </span>
              </div>
              <h3 className="font-semibold text-gray-900 dark:text-white text-lg">{stats.nextAppointment.patientName}</h3>
              <p className="text-gray-500 text-sm">{stats.nextAppointment.notes || "Follow-up"}</p>
            </div>
            
            <div className="flex gap-2 w-full md:w-auto mt-2 md:mt-0">
              <Button variant="outline" className="flex-1 md:flex-none">View Records</Button>
              <Button className="flex-1 md:flex-none bg-[var(--color-primary-600)] hover:bg-[var(--color-primary-700)]">Start Visit</Button>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
