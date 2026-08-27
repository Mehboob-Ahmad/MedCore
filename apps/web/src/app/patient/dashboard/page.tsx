"use client";

import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Calendar, Clock, MapPin, Search } from "lucide-react";
import Link from "next/link";

export default function PatientDashboard() {
  return (
    <div className="space-y-6 pb-20 md:pb-0">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Hello, Jane</h1>
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
          <Card className="bg-[var(--color-primary-600)] text-white border-none shadow-lg shadow-sky-900/10">
            <CardContent className="p-6">
              <h3 className="font-semibold text-sky-100 mb-1">Upcoming Appointment</h3>
              <div className="text-2xl font-bold mb-4">Tomorrow</div>
              <div className="space-y-2 text-sm text-sky-50">
                <div className="flex items-center gap-2">
                  <Clock className="w-4 h-4" />
                  <span>10:00 AM - 10:30 AM</span>
                </div>
                <div className="flex items-center gap-2">
                  <MapPin className="w-4 h-4" />
                  <span>Dr. Sarah Jenkins (Cardiology)</span>
                </div>
              </div>
            </CardContent>
          </Card>
        </motion.div>
        
        {/* Placeholder stats */}
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.2 }}>
          <Card>
            <CardContent className="p-6">
              <h3 className="font-medium text-gray-500 dark:text-gray-400 mb-1">Recent Test Results</h3>
              <div className="text-2xl font-bold text-gray-900 dark:text-white mb-2">2 New</div>
              <Button variant="outline" size="sm" className="w-full">View Results</Button>
            </CardContent>
          </Card>
        </motion.div>
        
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.3 }}>
          <Card>
            <CardContent className="p-6">
              <h3 className="font-medium text-gray-500 dark:text-gray-400 mb-1">Active Prescriptions</h3>
              <div className="text-2xl font-bold text-gray-900 dark:text-white mb-2">3</div>
              <Button variant="outline" size="sm" className="w-full">View Details</Button>
            </CardContent>
          </Card>
        </motion.div>
      </div>
    </div>
  );
}
