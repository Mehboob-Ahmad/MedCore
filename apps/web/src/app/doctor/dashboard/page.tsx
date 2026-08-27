"use client";

import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Users, Clock, Calendar, CheckCircle } from "lucide-react";

export default function DoctorDashboard() {
  return (
    <div className="space-y-6 pb-20 md:pb-0">
      <div>
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Dr. Smith's Dashboard</h1>
        <p className="text-gray-500 dark:text-gray-400">Here's your practice overview for today.</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.1 }}>
          <Card className="bg-indigo-600 text-white border-none shadow-lg shadow-indigo-900/10">
            <CardContent className="p-6">
              <h3 className="font-medium text-indigo-100 mb-1">Total Appointments</h3>
              <div className="text-3xl font-bold mb-2">12</div>
              <p className="text-sm text-indigo-200">Today</p>
            </CardContent>
          </Card>
        </motion.div>
        
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.2 }}>
          <Card>
            <CardContent className="p-6">
              <h3 className="font-medium text-gray-500 dark:text-gray-400 mb-1">Pending Requests</h3>
              <div className="text-3xl font-bold text-gray-900 dark:text-white mb-2">3</div>
              <p className="text-sm text-amber-500">Requires attention</p>
            </CardContent>
          </Card>
        </motion.div>

        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.3 }}>
          <Card>
            <CardContent className="p-6">
              <h3 className="font-medium text-gray-500 dark:text-gray-400 mb-1">New Messages</h3>
              <div className="text-3xl font-bold text-gray-900 dark:text-white mb-2">5</div>
              <p className="text-sm text-sky-500">From patients</p>
            </CardContent>
          </Card>
        </motion.div>
      </div>

      <h2 className="text-xl font-bold text-gray-900 dark:text-white mt-8 mb-4">Next Patient</h2>
      <Card>
        <CardContent className="p-6 flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <div className="flex items-center gap-2 mb-2">
              <span className="px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                Confirmed
              </span>
              <span className="flex items-center gap-1 text-xs text-gray-500 font-medium">
                <Clock className="w-3 h-3" /> 10:30 AM
              </span>
            </div>
            <h3 className="font-semibold text-gray-900 dark:text-white text-lg">Jane Doe</h3>
            <p className="text-gray-500 text-sm">Follow-up: Blood work analysis</p>
          </div>
          
          <div className="flex gap-2 w-full md:w-auto mt-2 md:mt-0">
            <Button variant="outline" className="flex-1 md:flex-none">View Records</Button>
            <Button className="flex-1 md:flex-none bg-indigo-600 hover:bg-indigo-700">Start Visit</Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
