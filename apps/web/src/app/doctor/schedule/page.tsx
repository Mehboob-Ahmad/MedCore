"use client";

import { useEffect, useState } from "react";
import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Calendar as CalendarIcon, Clock, Video, User, Loader2 } from "lucide-react";
import { DoctorService } from "@medichp/api-client";

export default function DoctorSchedule() {
  const [appointments, setAppointments] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchSchedule = async () => {
      try {
        const res = await DoctorService.getAppointments();
        if (res.success) {
          setAppointments(res.data);
        }
      } catch (error) {
        console.error("Failed to fetch schedule", error);
      } finally {
        setLoading(false);
      }
    };
    fetchSchedule();
  }, []);

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

      {loading ? (
        <div className="flex justify-center items-center h-32">
          <Loader2 className="w-8 h-8 animate-spin text-gray-500" />
        </div>
      ) : appointments.length === 0 ? (
        <Card className="text-center py-12">
          <CardContent>
            <p className="text-gray-500">No upcoming appointments scheduled.</p>
          </CardContent>
        </Card>
      ) : (
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
                        {apt.type || "Consultation"}
                      </span>
                    </div>
                    <h3 className="font-semibold text-gray-900 dark:text-white text-lg">{apt.patientName || "Patient"}</h3>
                    <p className="text-gray-500 text-sm">{apt.notes || "No additional notes"}</p>
                    
                    <div className="flex items-center gap-4 mt-4 text-sm text-gray-600 dark:text-gray-300">
                      <div className="flex items-center gap-1.5 font-medium text-indigo-600 dark:text-indigo-400">
                        <Clock className="w-4 h-4" />
                        {new Date(apt.dateTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                      </div>
                      <div className="flex items-center gap-1.5 text-gray-400">
                        <CalendarIcon className="w-4 h-4" />
                        {new Date(apt.dateTime).toLocaleDateString()}
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
                        <Button className="flex-1 md:flex-none bg-[var(--color-primary-600)] hover:bg-[var(--color-primary-700)]">Start Visit</Button>
                      </>
                    )}
                  </div>
                </CardContent>
              </Card>
            </motion.div>
          ))}
        </div>
      )}
    </div>
  );
}
