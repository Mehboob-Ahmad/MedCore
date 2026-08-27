"use client";

import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Calendar as CalendarIcon, Clock, Video, User } from "lucide-react";

export default function DoctorSchedule() {
  const appointments = [
    { id: 1, patient: "Jane Doe", reason: "Follow-up", time: "10:30 AM", status: "Confirmed", type: "Video Consult" },
    { id: 2, patient: "Mark Smith", reason: "Routine Checkup", time: "11:00 AM", status: "Pending", type: "In-Person" },
    { id: 3, patient: "Emily Johnson", reason: "Prescription Refill", time: "1:00 PM", status: "Confirmed", type: "Video Consult" },
  ];

  return (
    <div className="space-y-6 pb-20 md:pb-0">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Schedule</h1>
          <p className="text-gray-500 dark:text-gray-400">Manage your upcoming appointments and availability.</p>
        </div>
        <Button variant="outline" className="flex items-center gap-2">
          <CalendarIcon className="w-4 h-4" />
          Edit Availability
        </Button>
      </div>

      <div className="space-y-4">
        {appointments.map((apt, index) => (
          <motion.div key={apt.id} initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: index * 0.1 }}>
            <Card>
              <CardContent className="p-6 flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                  <div className="flex items-center gap-2 mb-2">
                    <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium ${apt.status === 'Confirmed' ? 'bg-green-100 text-green-800' : 'bg-amber-100 text-amber-800'}`}>
                      {apt.status}
                    </span>
                    <span className="flex items-center gap-1 text-xs text-gray-500 font-medium">
                      {apt.type === "Video Consult" ? <Video className="w-3 h-3" /> : <User className="w-3 h-3" />}
                      {apt.type}
                    </span>
                  </div>
                  <h3 className="font-semibold text-gray-900 dark:text-white text-lg">{apt.patient}</h3>
                  <p className="text-gray-500 text-sm">{apt.reason}</p>
                  
                  <div className="flex items-center gap-4 mt-4 text-sm text-gray-600 dark:text-gray-300">
                    <div className="flex items-center gap-1.5 font-medium text-indigo-600 dark:text-indigo-400">
                      <Clock className="w-4 h-4" />
                      {apt.time}
                    </div>
                  </div>
                </div>
                
                <div className="flex gap-2 w-full md:w-auto mt-2 md:mt-0">
                  {apt.status === "Pending" ? (
                    <>
                      <Button variant="outline" className="flex-1 md:flex-none">Decline</Button>
                      <Button className="flex-1 md:flex-none bg-indigo-600 hover:bg-indigo-700">Approve</Button>
                    </>
                  ) : (
                    <>
                      <Button variant="outline" className="flex-1 md:flex-none">Reschedule</Button>
                      <Button className="flex-1 md:flex-none bg-indigo-600 hover:bg-indigo-700">Start Visit</Button>
                    </>
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
