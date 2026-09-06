"use client";

import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { User, Mail, Phone, MapPin, FileBadge, DollarSign } from "lucide-react";
import { DoctorService } from "@medichp/api-client";
import { useAuth } from "@/contexts/AuthContext";

export default function DoctorProfile() {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState("");
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phoneNumber: "",
    specializations: [] as string[],
    clinicName: "",
    clinicAddress: "",
    consultationFee: 0,
    bio: ""
  });

  useEffect(() => {
    fetchProfile();
  }, []);

  const fetchProfile = async () => {
    try {
      setLoading(true);
      const res = await DoctorService.getProfile();
      if (res.success && res.data) {
        const p = res.data;
        setFormData({
          firstName: p.firstName || "",
          lastName: p.lastName || "",
          email: p.email || "",
          phoneNumber: p.phoneNumber || "",
          specializations: p.specializations || [],
          clinicName: p.clinicName || "",
          clinicAddress: p.clinicAddress || "",
          consultationFee: p.consultationFee || 0,
          bio: p.bio || ""
        });
      }
    } catch (err) {
      console.error("Failed to load profile:", err);
      setMessage("Failed to load profile.");
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setMessage("");
    try {
      const res = await DoctorService.updateProfile({
        Bio: formData.bio,
        ConsultationFee: Number(formData.consultationFee),
        ExperienceYears: 0, // Keeping this default for now
        WhatsAppNumber: formData.phoneNumber,
        WhatsAppEnabled: true,
        ClinicName: formData.clinicName,
        ClinicAddress: formData.clinicAddress,
        Specialization: formData.specializations[0] || "",
        Qualifications: [],
        Certifications: []
      });
      if (res.success) {
        setMessage("Profile updated successfully!");
      }
    } catch (err: any) {
      setMessage(err.message || "Failed to update profile.");
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return <div className="p-8 text-center text-gray-500">Loading profile...</div>;
  }

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
                <h2 className="text-xl font-bold text-gray-900 dark:text-white">Dr. {formData.firstName} {formData.lastName}</h2>
                <p className="text-gray-500">{formData.specializations.join(", ") || "General Practitioner"}</p>
              </div>
              <Button variant="outline" className="ml-auto">Update Photo</Button>
            </div>

            {message && (
              <div className={`p-4 mb-6 rounded-lg ${message.includes("success") ? "bg-green-50 text-green-700" : "bg-red-50 text-red-700"}`}>
                {message}
              </div>
            )}

            <form className="space-y-6" onSubmit={handleSubmit}>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">First Name (Readonly)</label>
                  <Input value={formData.firstName} readOnly className="bg-gray-50" />
                </div>
                
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Last Name (Readonly)</label>
                  <Input value={formData.lastName} readOnly className="bg-gray-50" />
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Email Address (Readonly)</label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input type="email" value={formData.email} readOnly className="pl-9 bg-gray-50" />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Phone Number</label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input type="tel" value={formData.phoneNumber} onChange={e => setFormData({...formData, phoneNumber: e.target.value})} className="pl-9" />
                  </div>
                </div>
                
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Clinic Name</label>
                  <Input value={formData.clinicName} onChange={e => setFormData({...formData, clinicName: e.target.value})} />
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Clinic Location</label>
                  <div className="relative">
                    <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input value={formData.clinicAddress} onChange={e => setFormData({...formData, clinicAddress: e.target.value})} className="pl-9" />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Consultation Fee</label>
                  <div className="relative">
                    <DollarSign className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input type="number" value={formData.consultationFee} onChange={e => setFormData({...formData, consultationFee: Number(e.target.value)})} className="pl-9" />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Specialization</label>
                  <Input value={formData.specializations[0] || ""} onChange={e => setFormData({...formData, specializations: [e.target.value]})} placeholder="e.g. Cardiologist" />
                </div>
              </div>

              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Bio / About</label>
                <textarea 
                  className="w-full min-h-[100px] p-3 rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-gray-900 dark:text-white text-sm focus:outline-none focus:ring-2 focus:ring-[var(--color-primary-600)] focus:border-transparent transition-shadow"
                  value={formData.bio}
                  onChange={e => setFormData({...formData, bio: e.target.value})}
                  placeholder="Tell patients about yourself..."
                />
              </div>

              <div className="pt-4 flex justify-end">
                <Button type="submit" disabled={saving}>
                  {saving ? "Saving..." : "Save Changes"}
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  );
}
