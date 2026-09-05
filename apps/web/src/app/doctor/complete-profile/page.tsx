"use client";

import { useState } from "react";
import { motion } from "framer-motion";
import { UserCircle, BriefcaseMedical, Phone, Building2, Clock, DollarSign, FileBadge, MapPin } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { useRouter } from "next/navigation";
import { DoctorService, SystemService } from "@medichp/api-client";
import { useEffect } from "react";

export default function CompleteProfilePage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [formData, setFormData] = useState({
    phoneNumber: "",
    licenseAuthority: "",
    registrationNumber: "123456", // Default placeholder
    clinicName: "",
    address: "Not specified",
    consultationFee: 1000,
    yearsOfExperience: 5, // Default placeholder
    availabilityHours: "",
    cityId: "",
  });
  const [cities, setCities] = useState<any[]>([]);

  useEffect(() => {
    const fetchCities = async () => {
      try {
        const res = await SystemService.getCities();
        if (res.success) {
          setCities(res.data || []);
        }
      } catch (err) {
        console.error("Failed to fetch cities", err);
      }
    };
    fetchCities();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      const payload = {
        ...formData,
        specializationIds: [],
        consultationFee: Number(formData.consultationFee),
        cityId: formData.cityId ? formData.cityId : undefined
      };
      await DoctorService.completeProfile(payload);
      router.push("/doctor/dashboard");
    } catch (err: any) {
      setError(err.message || "Failed to complete profile");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex-1 flex items-center justify-center p-4 bg-surface-50 dark:bg-slate-900 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-blue-100/40 via-transparent to-transparent dark:from-blue-900/10 py-12">
      <motion.div 
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="w-full max-w-2xl"
      >
        <Card className="border-0 shadow-2xl shadow-blue-900/5 dark:shadow-none dark:bg-slate-800/80 border-t-4 border-t-[var(--color-primary-500)]">
          <CardHeader className="text-center pb-8 border-b border-gray-100 dark:border-gray-800">
            <div className="mx-auto w-16 h-16 bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400 rounded-full flex items-center justify-center mb-4">
              <UserCircle size={32} />
            </div>
            <CardTitle className="text-2xl font-bold text-gray-900 dark:text-white">Complete Your Profile</CardTitle>
            <CardDescription className="text-gray-500 dark:text-gray-400 text-base mt-2">
              Tell us more about your practice to start accepting patients.
            </CardDescription>
          </CardHeader>
          <CardContent className="pt-8">
            {error && (
              <div className="mb-4 p-3 rounded bg-red-50 text-red-600 text-sm border border-red-200">
                {error}
              </div>
            )}
            <form className="space-y-6" onSubmit={handleSubmit}>
              
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Specialization</label>
                  <div className="relative">
                    <BriefcaseMedical className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input placeholder="e.g. Cardiology (Not connected to API)" className="pl-10" />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Contact Number</label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input type="tel" name="phoneNumber" value={formData.phoneNumber} onChange={handleChange} placeholder="+1 (555) 000-0000" className="pl-10" required />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Licensing Authority</label>
                  <div className="relative">
                    <FileBadge className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input name="licenseAuthority" value={formData.licenseAuthority} onChange={handleChange} placeholder="e.g. Medical Council" className="pl-10" required />
                  </div>
                </div>
                
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Clinic / Hospital Name</label>
                  <div className="relative">
                    <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input name="clinicName" value={formData.clinicName} onChange={handleChange} placeholder="City General Hospital" className="pl-10" required />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">City</label>
                  <div className="relative">
                    <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <select 
                      name="cityId"
                      value={formData.cityId}
                      onChange={(e) => setFormData({...formData, cityId: e.target.value})}
                      className="pl-10 h-10 w-full border border-gray-200 dark:border-slate-800 bg-white dark:bg-slate-900 rounded-md text-sm text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-[var(--color-primary-600)] focus:border-transparent transition-shadow outline-none"
                      required
                    >
                      <option value="">Select a City</option>
                      {cities.map(c => (
                        <option key={c.id} value={c.id}>{c.name}</option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Availability Hours</label>
                  <div className="relative">
                    <Clock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input name="availabilityHours" value={formData.availabilityHours} onChange={handleChange} placeholder="Mon-Fri, 9:00 AM - 5:00 PM" className="pl-10" required />
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Consultation Fee (PKR)</label>
                  <div className="relative">
                    <DollarSign className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input type="number" name="consultationFee" value={formData.consultationFee} onChange={handleChange} placeholder="1000" className="pl-10" required />
                  </div>
                </div>
              </div>

              <div className="pt-4 border-t border-gray-100 dark:border-gray-800 flex justify-end">
                <Button type="button" variant="outline" className="mr-3" onClick={() => router.push('/doctor/dashboard')}>
                  Skip for now
                </Button>
                <Button type="submit" className="bg-[var(--color-primary-600)] hover:bg-[var(--color-primary-700)] text-white px-8" disabled={loading}>
                  {loading ? "Saving..." : "Save Profile"}
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  );
}
