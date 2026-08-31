"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { Card, CardContent, CardHeader, CardTitle } from "@medichp/ui";
import { DoctorService } from "@medichp/api-client";
import { HeartPulse, Activity, CalendarDays, User, Stethoscope } from "lucide-react";

export default function PatientClinicalSummary() {
  const params = useParams();
  const patientId = params.id as string;
  const [summary, setSummary] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (patientId) {
      fetchSummary();
    }
  }, [patientId]);

  const fetchSummary = async () => {
    try {
      setLoading(true);
      const res = await DoctorService.getPatientClinicalSummary(patientId);
      if (res.success && res.data) {
        setSummary(res.data);
      }
    } catch (err: any) {
      setError(err.message || "Failed to load clinical summary");
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="p-8 text-center text-gray-500">Loading patient data...</div>;
  }

  if (error) {
    return <div className="p-8 text-center text-red-500">{error}</div>;
  }

  if (!summary) {
    return <div className="p-8 text-center text-gray-500">No clinical summary available.</div>;
  }

  return (
    <div className="space-y-6 max-w-5xl mx-auto pb-12">
      <div className="flex items-center gap-4 border-b border-gray-200 dark:border-slate-800 pb-6">
        <div className="w-16 h-16 rounded-full bg-sky-100 flex items-center justify-center text-[var(--color-primary-600)]">
          <User className="w-8 h-8" />
        </div>
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
            {summary.patientProfile?.firstName} {summary.patientProfile?.lastName}
          </h1>
          <p className="text-gray-500">Patient ID: {patientId}</p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {/* Personal Details */}
        <Card className="md:col-span-2 lg:col-span-3">
          <CardHeader>
            <CardTitle className="flex items-center gap-2"><User className="w-5 h-5 text-gray-500"/> Personal & General Health</CardTitle>
          </CardHeader>
          <CardContent className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <div>
              <p className="text-xs text-gray-500">Gender</p>
              <p className="font-medium text-gray-900 dark:text-gray-100">{summary.patientProfile?.gender || "N/A"}</p>
            </div>
            <div>
              <p className="text-xs text-gray-500">Blood Type</p>
              <p className="font-medium text-[var(--color-primary-600)]">{summary.patientProfile?.bloodType || "N/A"}</p>
            </div>
            <div>
              <p className="text-xs text-gray-500">Date of Birth</p>
              <p className="font-medium text-gray-900 dark:text-gray-100">
                {summary.patientProfile?.dateOfBirth ? new Date(summary.patientProfile.dateOfBirth).toLocaleDateString() : "N/A"}
              </p>
            </div>
          </CardContent>
        </Card>

        {/* Medical History */}
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center gap-2"><HeartPulse className="w-5 h-5 text-rose-500"/> Medical Histories</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div>
              <p className="text-sm font-semibold text-gray-700 dark:text-gray-300">General Medical History</p>
              <p className="text-sm text-gray-600 dark:text-gray-400 bg-gray-50 dark:bg-slate-800 p-3 rounded mt-1">
                {summary.patientProfile?.medicalHistory || "None recorded"}
              </p>
            </div>
            <div>
              <p className="text-sm font-semibold text-gray-700 dark:text-gray-300">Family Medical History</p>
              <p className="text-sm text-gray-600 dark:text-gray-400 bg-gray-50 dark:bg-slate-800 p-3 rounded mt-1">
                {summary.patientProfile?.familyMedicalHistory || "None recorded"}
              </p>
            </div>
            <div>
              <p className="text-sm font-semibold text-gray-700 dark:text-gray-300">Lifestyle</p>
              <p className="text-sm text-gray-600 dark:text-gray-400 bg-gray-50 dark:bg-slate-800 p-3 rounded mt-1">
                {summary.patientProfile?.lifestyleInformation || "None recorded"}
              </p>
            </div>
          </CardContent>
        </Card>

        <div className="space-y-6">
          {/* Allergies */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2"><Activity className="w-5 h-5 text-amber-500"/> Allergies</CardTitle>
            </CardHeader>
            <CardContent>
              {summary.patientProfile?.allergies?.length > 0 ? (
                <ul className="space-y-2">
                  {summary.patientProfile.allergies.map((a: any) => (
                    <li key={a.id} className="text-sm border-b border-gray-100 dark:border-slate-800 pb-2">
                      <span className="font-medium">{a.allergyName}</span>
                      {a.severity && <span className="ml-2 px-2 py-0.5 bg-amber-100 text-amber-700 text-xs rounded-full">{a.severity}</span>}
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="text-sm text-gray-500 italic">No allergies recorded.</p>
              )}
            </CardContent>
          </Card>

          {/* Chronic Conditions */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2"><Stethoscope className="w-5 h-5 text-[var(--color-primary-600)]"/> Chronic Conditions</CardTitle>
            </CardHeader>
            <CardContent>
              {summary.patientProfile?.chronicConditions?.length > 0 ? (
                <ul className="space-y-2">
                  {summary.patientProfile.chronicConditions.map((c: any) => (
                    <li key={c.id} className="text-sm border-b border-gray-100 dark:border-slate-800 pb-2">
                      <span className="font-medium">{c.conditionName}</span>
                      <p className="text-xs text-gray-500">{c.diagnosedDate && new Date(c.diagnosedDate).toLocaleDateString()}</p>
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="text-sm text-gray-500 italic">No chronic conditions recorded.</p>
              )}
            </CardContent>
          </Card>
        </div>

        {/* Procedures */}
        <Card className="lg:col-span-3">
          <CardHeader>
            <CardTitle className="flex items-center gap-2"><CalendarDays className="w-5 h-5 text-indigo-500"/> Surgeries & Hospitalizations</CardTitle>
          </CardHeader>
          <CardContent className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <h4 className="font-semibold text-gray-700 dark:text-gray-300 mb-3 text-sm">Surgeries</h4>
              {summary.patientProfile?.surgeries?.length > 0 ? (
                <div className="space-y-3">
                  {summary.patientProfile.surgeries.map((s: any) => (
                    <div key={s.id} className="bg-gray-50 dark:bg-slate-800 p-3 rounded text-sm">
                      <p className="font-medium">{s.surgeryName}</p>
                      <p className="text-xs text-gray-500">{s.surgeryDate && new Date(s.surgeryDate).toLocaleDateString()} @ {s.hospitalName || "Unknown"}</p>
                    </div>
                  ))}
                </div>
              ) : (
                <p className="text-sm text-gray-500 italic">No surgeries recorded.</p>
              )}
            </div>

            <div>
              <h4 className="font-semibold text-gray-700 dark:text-gray-300 mb-3 text-sm">Hospitalizations</h4>
              {summary.patientProfile?.hospitalizations?.length > 0 ? (
                <div className="space-y-3">
                  {summary.patientProfile.hospitalizations.map((h: any) => (
                    <div key={h.id} className="bg-gray-50 dark:bg-slate-800 p-3 rounded text-sm">
                      <p className="font-medium">{h.reason}</p>
                      <p className="text-xs text-gray-500">
                        {h.admissionDate && new Date(h.admissionDate).toLocaleDateString()} - {h.dischargeDate && new Date(h.dischargeDate).toLocaleDateString()}
                      </p>
                      <p className="text-xs text-gray-500">@ {h.hospitalName || "Unknown"}</p>
                    </div>
                  ))}
                </div>
              ) : (
                <p className="text-sm text-gray-500 italic">No hospitalizations recorded.</p>
              )}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
