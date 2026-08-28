"use client";

import { useEffect, useState } from "react";
import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Calendar as CalendarIcon, Clock, Video, Loader2 } from "lucide-react";
import { PatientService } from "@medichp/api-client";

export default function PatientAppointments() {
  const [appointments, setAppointments] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchAppointments = async () => {
      try {
        const res = await PatientService.getAppointments();
        if (res.success) {
          setAppointments(res.data);
        }
      } catch (error) {
        console.error("Failed to fetch appointments", error);
      } finally {
        setLoading(false);
      }
    };
    fetchAppointments();
  }, []);

  return (
    <div className="space-y-6 pb-20 md:pb-0">
      <div>
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Appointments</h1>
        <p className="text-gray-500 dark:text-gray-400">Manage your past and upcoming consultations.</p>
      </div>

      {loading ? (
        <div className="flex justify-center items-center h-32">
          <Loader2 className="w-8 h-8 animate-spin text-gray-500" />
        </div>
      ) : appointments.length === 0 ? (
        <Card className="text-center py-12">
          <CardContent>
            <p className="text-gray-500">No appointments found.</p>
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
                      <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium ${apt.status === 'Upcoming' ? 'bg-sky-100 text-sky-800' : 'bg-green-100 text-green-800'}`}>
                        {apt.status}
                      </span>
                      <span className="flex items-center gap-1 text-xs text-gray-500 font-medium">
                        <Video className="w-3 h-3" /> {apt.type || "Consultation"}
                      </span>
                    </div>
                    <h3 className="font-semibold text-gray-900 dark:text-white text-lg">{apt.doctorName || "Dr. Unassigned"}</h3>
                    <p className="text-gray-500 text-sm">{apt.specialty || "General"}</p>
                    
                    <div className="flex items-center gap-4 mt-4 text-sm text-gray-600 dark:text-gray-300">
                      <div className="flex items-center gap-1.5">
                        <CalendarIcon className="w-4 h-4 text-gray-400" />
                        {new Date(apt.dateTime).toLocaleDateString()}
                      </div>
                      <div className="flex items-center gap-1.5">
                        <Clock className="w-4 h-4 text-gray-400" />
                        {new Date(apt.dateTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
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
                    {(apt.status === "Completed" || apt.status === "Done") && (
                      <Button variant="outline" className="w-full md:w-auto">View Summary</Button>
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
