"use client";

import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { User, Mail, Phone, MapPin, FileBadge, DollarSign } from "lucide-react";

export default function DoctorProfile() {
  return (
    <div className="space-y-6 pb-20 md:pb-0 max-w-3xl mx-auto">
      <div>
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Professional Profile</h1>
        <p className="text-gray-500 dark:text-gray-400">Manage your practice details and consultation fees.</p>
      </div>

      <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }}>
        <Card>
          <CardContent className="p-6">
            <div className="flex items-center gap-4 mb-8 pb-8 border-b border-gray-100 dark:border-slate-800">
              <div className="w-20 h-20 bg-indigo-100 dark:bg-indigo-900/50 rounded-full flex items-center justify-center text-indigo-600">
                <User className="w-8 h-8" />
              </div>
              <div>
                <h2 className="text-xl font-bold text-gray-900 dark:text-white">Dr. Sarah Jenkins</h2>
                <p className="text-gray-500">Cardiology Specialist</p>
              </div>
              <Button variant="outline" className="ml-auto">Update Photo</Button>
            </div>

            <form className="space-y-6" onSubmit={e => e.preventDefault()}>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Full Name</label>
                  <Input defaultValue="Dr. Sarah Jenkins" />
                </div>
                
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Specialization</label>
                  <div className="relative">
                    <FileBadge className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input defaultValue="Cardiology" className="pl-9" />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Email Address</label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input type="email" defaultValue="sarah.jenkins@clinic.com" className="pl-9" />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Phone Number</label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input type="tel" defaultValue="+1 (555) 123-4567" className="pl-9" />
                  </div>
                </div>
                
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Clinic Location</label>
                  <div className="relative">
                    <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input defaultValue="123 Health Ave, NY" className="pl-9" />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Consultation Fee</label>
                  <div className="relative">
                    <DollarSign className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input type="number" defaultValue="150" className="pl-9" />
                  </div>
                </div>
              </div>

              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Bio / About</label>
                <textarea 
                  className="w-full min-h-[100px] p-3 rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-gray-900 dark:text-white text-sm focus:outline-none focus:ring-2 focus:ring-indigo-600 focus:border-transparent transition-shadow"
                  defaultValue="Dr. Sarah Jenkins is a board-certified cardiologist with over 15 years of experience in diagnosing and treating cardiovascular diseases."
                />
              </div>

              <div className="pt-4 flex justify-end">
                <Button className="bg-indigo-600 hover:bg-indigo-700">Save Changes</Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  );
}
