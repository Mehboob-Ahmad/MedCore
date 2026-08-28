"use client";

import { useEffect, useState } from "react";
import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { Users, Activity, Mail, Loader2 } from "lucide-react";
import { AdminService, AuthService } from "@medichp/api-client";

export default function AdminDashboard() {
  const [inviteForm, setInviteForm] = useState({
    email: "",
    firstName: "",
    lastName: "",
    phoneNumber: ""
  });
  const [loading, setLoading] = useState(false);
  const [statsLoading, setStatsLoading] = useState(true);
  const [stats, setStats] = useState({
    totalUsers: 0,
    totalDoctors: 0,
    totalPatients: 0,
    monthlyActive: 0,
  });

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const response = await AdminService.getStats();
        if (response.success) {
          setStats(response.data);
        }
      } catch (error) {
        console.error("Failed to fetch admin stats", error);
      } finally {
        setStatsLoading(false);
      }
    };
    fetchStats();
  }, []);

  const handleInvite = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!inviteForm.email) return;
    
    setLoading(true);
    try {
      await AuthService.inviteAdmin(inviteForm);
      alert(`Admin invitation sent to ${inviteForm.email}`);
      setInviteForm({ email: "", firstName: "", lastName: "", phoneNumber: "" });
    } catch (error: any) {
      alert(error.response?.data?.message || "Failed to send invitation.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="space-y-8 pb-20 md:pb-0 max-w-5xl mx-auto">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Admin Overview</h1>
          <p className="text-gray-500 dark:text-gray-400">System status and administrative actions.</p>
        </div>
        <Button variant="outline" className="border-red-200 text-red-600 hover:bg-red-50 hover:text-red-700">
          Export Logs
        </Button>
      </div>

      {statsLoading ? (
        <div className="flex justify-center items-center h-48">
          <Loader2 className="w-8 h-8 animate-spin text-gray-500" />
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.1 }}>
            <Card>
              <CardContent className="p-6">
                <h3 className="font-medium text-gray-500 dark:text-gray-400 mb-1">Total Users</h3>
                <div className="text-3xl font-bold text-gray-900 dark:text-white mb-2">{stats.totalUsers}</div>
                <p className="text-sm text-green-500 flex items-center gap-1"><Activity className="w-3 h-3" /> Live</p>
              </CardContent>
            </Card>
          </motion.div>
          
          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.2 }}>
            <Card>
              <CardContent className="p-6">
                <h3 className="font-medium text-gray-500 dark:text-gray-400 mb-1">Monthly Active</h3>
                <div className="text-3xl font-bold text-gray-900 dark:text-white mb-2">{stats.monthlyActive}</div>
                <p className="text-sm text-green-500 flex items-center gap-1"><Activity className="w-3 h-3" /> Live</p>
              </CardContent>
            </Card>
          </motion.div>

          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.3 }}>
            <Card className="bg-[var(--color-primary-600)] text-white border-none shadow-lg shadow-sky-900/10">
              <CardContent className="p-6">
                <h3 className="font-medium text-sky-100 mb-1">Total Doctors</h3>
                <div className="text-3xl font-bold mb-2">{stats.totalDoctors}</div>
                <p className="text-sm text-sky-200">Registered practitioners</p>
              </CardContent>
            </Card>
          </motion.div>

          <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.4 }}>
            <Card className="bg-[var(--color-secondary-500)] text-white border-none shadow-lg shadow-green-900/10">
              <CardContent className="p-6">
                <h3 className="font-medium text-green-100 mb-1">Total Patients</h3>
                <div className="text-3xl font-bold mb-2">{stats.totalPatients}</div>
                <p className="text-sm text-green-200">Registered patients</p>
              </CardContent>
            </Card>
          </motion.div>
        </div>
      )}

      <h2 className="text-xl font-bold text-gray-900 dark:text-white pt-4">System Administration</h2>
      <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.5 }}>
        <Card>
          <CardContent className="p-6 max-w-xl">
            <h3 className="font-bold text-lg text-gray-900 dark:text-white mb-2">Invite New Super Admin</h3>
            <p className="text-gray-500 dark:text-gray-400 text-sm mb-6">
              Enter the email address of the person you want to invite. They will receive an email with instructions and a temporary password.
            </p>
            
            <form onSubmit={handleInvite} className="space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="relative">
                  <Input 
                    type="text" 
                    placeholder="First Name" 
                    value={inviteForm.firstName}
                    onChange={(e) => setInviteForm({...inviteForm, firstName: e.target.value})}
                    required
                  />
                </div>
                <div className="relative">
                  <Input 
                    type="text" 
                    placeholder="Last Name" 
                    value={inviteForm.lastName}
                    onChange={(e) => setInviteForm({...inviteForm, lastName: e.target.value})}
                    required
                  />
                </div>
              </div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="relative">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input 
                    type="email" 
                    placeholder="admin@example.com" 
                    className="pl-10" 
                    value={inviteForm.email}
                    onChange={(e) => setInviteForm({...inviteForm, email: e.target.value})}
                    required
                  />
                </div>
                <div className="relative">
                  <Input 
                    type="tel" 
                    placeholder="Phone Number" 
                    value={inviteForm.phoneNumber}
                    onChange={(e) => setInviteForm({...inviteForm, phoneNumber: e.target.value})}
                    required
                  />
                </div>
              </div>
              <Button type="submit" disabled={loading || !inviteForm.email} className="w-full sm:w-auto bg-slate-900 hover:bg-slate-800 dark:bg-slate-700">
                {loading ? "Sending..." : "Send Invitation"}
              </Button>
            </form>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  );
}
