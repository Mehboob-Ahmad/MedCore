"use client";

import { useState, useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { User, Mail, Phone, MapPin, Stethoscope, HeartPulse, Activity, CalendarPlus, X } from "lucide-react";
import { PatientService, AuthService } from "@medichp/api-client";
import { useAuth } from "@/contexts/AuthContext";
import { FileText, UploadCloud, File as FileIcon } from "lucide-react";

export default function PatientProfile() {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState("personal");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState("");
  const [reports, setReports] = useState<any[]>([]);
  const [uploadingReport, setUploadingReport] = useState(false);
  
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phoneNumber: "",
    address: "",
    gender: "",
    bloodType: "",
    familyMedicalHistory: "",
    medicalHistory: "",
    immunizationHistory: "",
    lifestyleInformation: "",
    surgeries: [] as any[],
    hospitalizations: [] as any[]
  });

  useEffect(() => {
    fetchProfile();
  }, []);

  const fetchProfile = async () => {
    try {
      setLoading(true);
      const res = await PatientService.getProfile();
      if (res.success && res.data) {
        const p = res.data;
        setFormData({
          firstName: p.firstName || "",
          lastName: p.lastName || "",
          email: p.email || "",
          phoneNumber: p.phoneNumber || "",
          address: p.address || "",
          gender: p.gender || "",
          bloodType: p.bloodType || "",
          familyMedicalHistory: p.familyMedicalHistory || "",
          medicalHistory: p.medicalHistory || "",
          immunizationHistory: p.immunizationHistory || "",
          lifestyleInformation: p.lifestyleInformation || "",
          surgeries: p.surgeries || [],
          hospitalizations: p.hospitalizations || []
        });
      }
      const reportsRes = await PatientService.getReports();
      if (reportsRes.success) {
        setReports(reportsRes.data);
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
      const res = await PatientService.updateProfile({
        Address: formData.address,
        Gender: formData.gender,
        BloodType: formData.bloodType,
        FamilyMedicalHistory: formData.familyMedicalHistory,
        MedicalHistory: formData.medicalHistory,
        ImmunizationHistory: formData.immunizationHistory,
        LifestyleInformation: formData.lifestyleInformation,
        Surgeries: formData.surgeries,
        Hospitalizations: formData.hospitalizations
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

  const addSurgery = () => {
    setFormData(prev => ({
      ...prev,
      surgeries: [...prev.surgeries, { surgeryName: "", surgeryDate: "", hospitalName: "", notes: "" }]
    }));
  };

  const removeSurgery = (index: number) => {
    setFormData(prev => ({
      ...prev,
      surgeries: prev.surgeries.filter((_, i) => i !== index)
    }));
  };

  const updateSurgery = (index: number, field: string, value: string) => {
    setFormData(prev => {
      const newSurgeries = [...prev.surgeries];
      newSurgeries[index] = { ...newSurgeries[index], [field]: value };
      return { ...prev, surgeries: newSurgeries };
    });
  };

  const addHospitalization = () => {
    setFormData(prev => ({
      ...prev,
      hospitalizations: [...prev.hospitalizations, { reason: "", admissionDate: "", dischargeDate: "", hospitalName: "", notes: "" }]
    }));
  };

  const removeHospitalization = (index: number) => {
    setFormData(prev => ({
      ...prev,
      hospitalizations: prev.hospitalizations.filter((_, i) => i !== index)
    }));
  };

  const updateHospitalization = (index: number, field: string, value: string) => {
    setFormData(prev => {
      const newHosp = [...prev.hospitalizations];
      newHosp[index] = { ...newHosp[index], [field]: value };
      return { ...prev, hospitalizations: newHosp };
    });
  };

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    try {
      setUploadingReport(true);
      const res = await AuthService.uploadFile(file, "MedicalReport");
      if (res.success) {
        // Refresh reports
        const reportsRes = await PatientService.getReports();
        if (reportsRes.success) setReports(reportsRes.data);
        setMessage("Report uploaded successfully!");
      }
    } catch (err: any) {
      setMessage(err.message || "Failed to upload report.");
    } finally {
      setUploadingReport(false);
    }
  };

  if (loading) {
    return <div className="p-8 text-center text-gray-500">Loading profile...</div>;
  }

  return (
    <div className="space-y-6 pb-20 md:pb-0 max-w-4xl mx-auto">
      <div>
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">My Profile</h1>
        <p className="text-gray-500 dark:text-gray-400">Manage your personal and health information.</p>
      </div>

      <div className="flex space-x-2 border-b border-gray-200 dark:border-slate-700 overflow-x-auto scrollbar-hide pb-2">
        <button
          onClick={() => setActiveTab("personal")}
          className={`px-4 py-2 font-medium text-sm rounded-t-lg transition-colors whitespace-nowrap ${
            activeTab === "personal" 
              ? "text-[var(--color-primary-600)] border-b-2 border-[var(--color-primary-600)]" 
              : "text-gray-500 hover:text-gray-700 dark:hover:text-gray-300"
          }`}
        >
          <div className="flex items-center gap-2"><User className="w-4 h-4" /> Personal Info</div>
        </button>
        <button
          onClick={() => setActiveTab("medical")}
          className={`px-4 py-2 font-medium text-sm rounded-t-lg transition-colors whitespace-nowrap ${
            activeTab === "medical" 
              ? "text-[var(--color-primary-600)] border-b-2 border-[var(--color-primary-600)]" 
              : "text-gray-500 hover:text-gray-700 dark:hover:text-gray-300"
          }`}
        >
          <div className="flex items-center gap-2"><HeartPulse className="w-4 h-4" /> Medical History</div>
        </button>
        <button
          onClick={() => setActiveTab("procedures")}
          className={`px-4 py-2 font-medium text-sm rounded-t-lg transition-colors whitespace-nowrap ${
            activeTab === "procedures" 
              ? "text-[var(--color-primary-600)] border-b-2 border-[var(--color-primary-600)]" 
              : "text-gray-500 hover:text-gray-700 dark:hover:text-gray-300"
          }`}
        >
          <div className="flex items-center gap-2"><Activity className="w-4 h-4" /> Surgeries & Hospitalizations</div>
        </button>
        <button
          onClick={() => setActiveTab("reports")}
          className={`px-4 py-2 font-medium text-sm rounded-t-lg transition-colors whitespace-nowrap ${
            activeTab === "reports" 
              ? "text-[var(--color-primary-600)] border-b-2 border-[var(--color-primary-600)]" 
              : "text-gray-500 hover:text-gray-700 dark:hover:text-gray-300"
          }`}
        >
          <div className="flex items-center gap-2"><FileText className="w-4 h-4" /> Medical Reports</div>
        </button>
      </div>

      {message && (
        <div className={`p-4 rounded-lg ${message.includes("success") ? "bg-green-50 text-green-700 border border-green-200" : "bg-red-50 text-red-700 border border-red-200"}`}>
          {message}
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-6">
        <AnimatePresence mode="wait">
          {activeTab === "personal" && (
            <motion.div key="personal" initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }}>
              <Card>
                <CardContent className="p-6">
                  <div className="flex items-center gap-4 mb-8 pb-8 border-b border-gray-100 dark:border-slate-800">
                    <div className="w-20 h-20 bg-sky-100 dark:bg-sky-900/50 rounded-full flex items-center justify-center text-[var(--color-primary-600)]">
                      <User className="w-8 h-8" />
                    </div>
                    <div>
                      <h2 className="text-xl font-bold text-gray-900 dark:text-white">{formData.firstName} {formData.lastName}</h2>
                      <p className="text-gray-500">Patient File</p>
                    </div>
                  </div>

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
                      <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Phone Number (Readonly)</label>
                      <div className="relative">
                        <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                        <Input type="tel" value={formData.phoneNumber} readOnly className="pl-9 bg-gray-50" />
                      </div>
                    </div>
                    <div className="space-y-1 md:col-span-2">
                      <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Address</label>
                      <div className="relative">
                        <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                        <Input value={formData.address} onChange={e => setFormData({...formData, address: e.target.value})} className="pl-9" />
                      </div>
                    </div>
                    <div className="space-y-1">
                      <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Gender</label>
                      <select 
                        value={formData.gender} 
                        onChange={e => setFormData({...formData, gender: e.target.value})}
                        className="w-full h-11 px-3 rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-sm focus:ring-[var(--color-primary-600)] focus:border-[var(--color-primary-600)]"
                      >
                        <option value="">Select Gender</option>
                        <option value="Male">Male</option>
                        <option value="Female">Female</option>
                        <option value="Other">Other</option>
                      </select>
                    </div>
                    <div className="space-y-1">
                      <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Blood Type</label>
                      <select 
                        value={formData.bloodType} 
                        onChange={e => setFormData({...formData, bloodType: e.target.value})}
                        className="w-full h-11 px-3 rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-sm focus:ring-[var(--color-primary-600)] focus:border-[var(--color-primary-600)]"
                      >
                        <option value="">Select Blood Type</option>
                        {["A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"].map(bt => <option key={bt} value={bt}>{bt}</option>)}
                      </select>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </motion.div>
          )}

          {activeTab === "medical" && (
            <motion.div key="medical" initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }}>
              <Card>
                <CardContent className="p-6 space-y-6">
                  <div className="space-y-2">
                    <label className="font-semibold text-gray-900 dark:text-white flex items-center gap-2">
                      <Stethoscope className="w-5 h-5 text-[var(--color-primary-600)]" /> General Medical History
                    </label>
                    <textarea 
                      className="w-full min-h-[100px] p-3 rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-sm"
                      placeholder="Describe past conditions, allergies, or chronic diseases..."
                      value={formData.medicalHistory}
                      onChange={e => setFormData({...formData, medicalHistory: e.target.value})}
                    />
                  </div>
                  
                  <div className="space-y-2">
                    <label className="font-semibold text-gray-900 dark:text-white flex items-center gap-2">
                      <User className="w-5 h-5 text-[var(--color-primary-600)]" /> Family Medical History
                    </label>
                    <textarea 
                      className="w-full min-h-[100px] p-3 rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-sm"
                      placeholder="Describe relevant family health history (e.g. Heart disease, Diabetes)..."
                      value={formData.familyMedicalHistory}
                      onChange={e => setFormData({...formData, familyMedicalHistory: e.target.value})}
                    />
                  </div>

                  <div className="space-y-2">
                    <label className="font-semibold text-gray-900 dark:text-white flex items-center gap-2">
                      <Activity className="w-5 h-5 text-[var(--color-primary-600)]" /> Lifestyle Information
                    </label>
                    <textarea 
                      className="w-full min-h-[100px] p-3 rounded-lg border border-gray-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-sm"
                      placeholder="Smoking, alcohol consumption, diet, exercise habits..."
                      value={formData.lifestyleInformation}
                      onChange={e => setFormData({...formData, lifestyleInformation: e.target.value})}
                    />
                  </div>
                </CardContent>
              </Card>
            </motion.div>
          )}

          {activeTab === "procedures" && (
            <motion.div key="procedures" initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }} className="space-y-6">
              {/* Surgeries Section */}
              <Card>
                <CardContent className="p-6">
                  <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
                    <h3 className="text-lg font-bold text-gray-900 dark:text-white flex items-center gap-2">
                      <Activity className="w-5 h-5 text-rose-500" /> Surgeries
                    </h3>
                    <Button type="button" variant="outline" size="sm" onClick={addSurgery} className="w-full sm:w-auto">
                      <CalendarPlus className="w-4 h-4 mr-2" /> Add Surgery
                    </Button>
                  </div>
                  
                  {formData.surgeries.length === 0 ? (
                    <p className="text-gray-500 italic text-sm text-center py-4">No surgeries recorded.</p>
                  ) : (
                    <div className="space-y-6">
                      {formData.surgeries.map((surgery, index) => (
                        <div key={index} className="p-4 bg-gray-50 dark:bg-slate-800/50 rounded-lg border border-gray-100 dark:border-slate-800 relative">
                          <button type="button" onClick={() => removeSurgery(index)} className="absolute top-4 right-4 text-gray-400 hover:text-red-500">
                            <X className="w-5 h-5" />
                          </button>
                          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pr-8">
                            <div className="space-y-1">
                              <label className="text-xs font-medium text-gray-500">Procedure Name</label>
                              <Input value={surgery.surgeryName} onChange={e => updateSurgery(index, "surgeryName", e.target.value)} placeholder="e.g. Appendectomy" />
                            </div>
                            <div className="space-y-1">
                              <label className="text-xs font-medium text-gray-500">Date (YYYY-MM-DD)</label>
                              <Input type="date" value={surgery.surgeryDate ? surgery.surgeryDate.split('T')[0] : ""} onChange={e => updateSurgery(index, "surgeryDate", e.target.value)} />
                            </div>
                            <div className="space-y-1">
                              <label className="text-xs font-medium text-gray-500">Hospital/Clinic Name</label>
                              <Input value={surgery.hospitalName || ""} onChange={e => updateSurgery(index, "hospitalName", e.target.value)} placeholder="e.g. General Hospital" />
                            </div>
                            <div className="space-y-1">
                              <label className="text-xs font-medium text-gray-500">Notes/Complications</label>
                              <Input value={surgery.notes || ""} onChange={e => updateSurgery(index, "notes", e.target.value)} placeholder="Optional notes" />
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </CardContent>
              </Card>

              {/* Hospitalizations Section */}
              <Card>
                <CardContent className="p-6">
                  <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
                    <h3 className="text-lg font-bold text-gray-900 dark:text-white flex items-center gap-2">
                      <MapPin className="w-5 h-5 text-blue-500" /> Hospitalizations
                    </h3>
                    <Button type="button" variant="outline" size="sm" onClick={addHospitalization} className="w-full sm:w-auto">
                      <CalendarPlus className="w-4 h-4 mr-2" /> Add Hospitalization
                    </Button>
                  </div>
                  
                  {formData.hospitalizations.length === 0 ? (
                    <p className="text-gray-500 italic text-sm text-center py-4">No hospitalizations recorded.</p>
                  ) : (
                    <div className="space-y-6">
                      {formData.hospitalizations.map((hosp, index) => (
                        <div key={index} className="p-4 bg-gray-50 dark:bg-slate-800/50 rounded-lg border border-gray-100 dark:border-slate-800 relative">
                          <button type="button" onClick={() => removeHospitalization(index)} className="absolute top-4 right-4 text-gray-400 hover:text-red-500">
                            <X className="w-5 h-5" />
                          </button>
                          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 pr-8">
                            <div className="space-y-1 md:col-span-2">
                              <label className="text-xs font-medium text-gray-500">Reason for admission</label>
                              <Input value={hosp.reason} onChange={e => updateHospitalization(index, "reason", e.target.value)} placeholder="e.g. Severe pneumonia" />
                            </div>
                            <div className="space-y-1">
                              <label className="text-xs font-medium text-gray-500">Admission Date</label>
                              <Input type="date" value={hosp.admissionDate ? hosp.admissionDate.split('T')[0] : ""} onChange={e => updateHospitalization(index, "admissionDate", e.target.value)} />
                            </div>
                            <div className="space-y-1">
                              <label className="text-xs font-medium text-gray-500">Discharge Date</label>
                              <Input type="date" value={hosp.dischargeDate ? hosp.dischargeDate.split('T')[0] : ""} onChange={e => updateHospitalization(index, "dischargeDate", e.target.value)} />
                            </div>
                            <div className="space-y-1 md:col-span-2">
                              <label className="text-xs font-medium text-gray-500">Hospital Name</label>
                              <Input value={hosp.hospitalName || ""} onChange={e => updateHospitalization(index, "hospitalName", e.target.value)} placeholder="e.g. City Hospital" />
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </CardContent>
              </Card>
            </motion.div>
          )}

          {activeTab === "reports" && (
            <motion.div key="reports" initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }} className="space-y-6">
              <Card>
                <CardContent className="p-6">
                  <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
                    <h3 className="text-lg font-bold text-gray-900 dark:text-white flex items-center gap-2">
                      <FileText className="w-5 h-5 text-[var(--color-primary-600)]" /> Uploaded Reports
                    </h3>
                    <div className="relative w-full sm:w-auto">
                      <input 
                        type="file" 
                        accept=".pdf,image/png,image/jpeg"
                        onChange={handleFileUpload} 
                        className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
                        disabled={uploadingReport}
                      />
                      <Button type="button" variant="outline" size="sm" disabled={uploadingReport} className="w-full sm:w-auto">
                        <UploadCloud className="w-4 h-4 mr-2" /> 
                        {uploadingReport ? "Uploading..." : "Upload Report"}
                      </Button>
                    </div>
                  </div>
                  
                  {reports.length === 0 ? (
                    <div className="text-center py-12 bg-gray-50 dark:bg-slate-800/50 rounded-lg border border-dashed border-gray-200 dark:border-slate-700">
                      <FileText className="w-12 h-12 text-gray-300 dark:text-gray-600 mx-auto mb-3" />
                      <p className="text-gray-500 text-sm">No medical reports uploaded yet.</p>
                    </div>
                  ) : (
                    <div className="space-y-3">
                      {reports.map((report) => (
                        <div key={report.id} className="flex items-center justify-between p-4 bg-gray-50 dark:bg-slate-800/50 rounded-lg border border-gray-100 dark:border-slate-800">
                          <div className="flex items-center gap-3">
                            <div className="w-10 h-10 rounded bg-white dark:bg-slate-700 flex items-center justify-center border border-gray-100 dark:border-slate-600 text-gray-400">
                              <FileIcon className="w-5 h-5" />
                            </div>
                            <div>
                              <h4 className="text-sm font-medium text-gray-900 dark:text-white line-clamp-1">{report.fileName}</h4>
                              <p className="text-xs text-gray-500 flex items-center gap-2">
                                <span>{new Date(report.createdAt).toLocaleDateString()}</span>
                                <span>•</span>
                                <span>{(report.fileSizeBytes / 1024 / 1024).toFixed(2)} MB</span>
                              </p>
                            </div>
                          </div>
                          <a 
                            href={`https://medichp.onrender.com${report.storagePath}`} 
                            target="_blank" 
                            rel="noopener noreferrer"
                            className="text-sm font-medium text-[var(--color-primary-600)] hover:underline"
                          >
                            View
                          </a>
                        </div>
                      ))}
                    </div>
                  )}
                </CardContent>
              </Card>
            </motion.div>
          )}
        </AnimatePresence>

        <div className="flex justify-end pt-4">
          <Button type="submit" disabled={saving} size="lg">
            {saving ? "Saving Changes..." : "Save Profile"}
          </Button>
        </div>
      </form>
    </div>
  );
}
