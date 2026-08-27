"use client";

import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Calendar as CalendarIcon, Clock, Video } from "lucide-react";

export default function PatientAppointments() {
  const appointments = [
    { id: 1, doctor: "Dr. Sarah Jenkins", specialty: "Cardiologist", date: "Oct 24, 2026", time: "10:00 AM", status: "Upcoming", type: "Video Consult" },
    { id: 2, doctor: "Dr. Mike Ross", specialty: "General Physician", date: "Sep 15, 2026", time: "2:30 PM", status: "Completed", type: "In-Person" },
  ];

  return (
    <div className="space-y-6 pb-20 md:pb-0">
      <div>
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Appointments</h1>
        <p className="text-gray-500 dark:text-gray-400">Manage your past and upcoming consultations.</p>
      </div>

      <div className="space-y-4">
        {appointments.map((apt, index) => (
          <motion.div key={apt.id} initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: index * 0.1 }}>
            <Card>
              <CardContent className="p-6 flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                  <div className="flex items-center gap-2 mb-2">
                    <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium ${apt.status === 'Upcoming' ? 'bg-sky-100 text-sky-800' : 'bg-green-100 text-green-800'}`}>
                      {apt.status}
                    </span>
                    <span className="flex items-center gap-1 text-xs text-gray-500 font-medium">
                      <Video className="w-3 h-3" /> {apt.type}
                    </span>
                  </div>
                  <h3 className="font-semibold text-gray-900 dark:text-white text-lg">{apt.doctor}</h3>
                  <p className="text-gray-500 text-sm">{apt.specialty}</p>
                  
                  <div className="flex items-center gap-4 mt-4 text-sm text-gray-600 dark:text-gray-300">
                    <div className="flex items-center gap-1.5">
                      <CalendarIcon className="w-4 h-4 text-gray-400" />
                      {apt.date}
                    </div>
                    <div className="flex items-center gap-1.5">
                      <Clock className="w-4 h-4 text-gray-400" />
                      {apt.time}
                    </div>
                  </div>
                </div>
                
                <div className="flex gap-2 w-full md:w-auto mt-2 md:mt-0">
                  {apt.status === "Upcoming" && (
                    <>
                      <Button variant="outline" className="flex-1 md:flex-none">Reschedule</Button>
                      <Button className="flex-1 md:flex-none bg-[var(--color-secondary-500)] hover:bg-[var(--color-secondary-600)]">Join Call</Button>
                    </>
                  )}
                  {apt.status === "Completed" && (
                    <Button variant="outline" className="w-full md:w-auto">View Summary</Button>
                  )}
                </div>
              </CardContent>
            </Card>
          </motion.div>
        ))}
      </div>
    </div>
  );
}
