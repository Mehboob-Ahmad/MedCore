"use client";

import { useState } from "react";
import { PublicService, AuthService } from "@medichp/api-client";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { FileUp, Loader2, CheckCircle2 } from "lucide-react";
import Link from "next/link";

export default function RequestDemoPage() {
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [degreeFile, setDegreeFile] = useState<File | null>(null);
  const [licenseFile, setLicenseFile] = useState<File | null>(null);

  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    phoneNumber: "",
    city: "",
    clinicOrHospital: "",
    yearsOfExperience: 0,
    specialization: "",
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === "yearsOfExperience" ? parseInt(value) || 0 : value,
    }));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>, type: "degree" | "license") => {
    if (e.target.files && e.target.files.length > 0) {
      if (type === "degree") setDegreeFile(e.target.files[0]);
      if (type === "license") setLicenseFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!degreeFile || !licenseFile) {
      alert("Please upload both your Degree and License images.");
      return;
    }

    setLoading(true);
    try {
      // 1. Upload images
      const degreeRes = await AuthService.uploadFile(degreeFile, "Verification");
      const licenseRes = await AuthService.uploadFile(licenseFile, "Verification");

      // 2. Submit form with URLs
      await PublicService.submitDemoRequest({
        ...formData,
        professionalQualification: "See Attached",
        degreeImageUrl: degreeRes.url || degreeRes.data?.url || "", // Adjust based on your API response
        licenseImageUrl: licenseRes.url || licenseRes.data?.url || "", 
      });

      setSuccess(true);
    } catch (err: any) {
      console.error(err);
      alert(err.message || "An error occurred while submitting your request.");
    } finally {
      setLoading(false);
    }
  };

  if (success) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-slate-900 p-4">
        <Card className="max-w-md w-full text-center">
          <CardContent className="p-8 space-y-6">
            <CheckCircle2 className="w-16 h-16 text-green-500 mx-auto" />
            <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Request Submitted!</h1>
            <p className="text-gray-500 dark:text-gray-400">
              Thank you for your interest in MediCore. Our team will review your application and contact you soon to set up your production doctor account.
            </p>
            <Link href="/">
              <Button className="w-full bg-[var(--color-primary-600)]">Return to Homepage</Button>
            </Link>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-slate-900 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-3xl mx-auto">
        <div className="text-center mb-10">
          <h1 className="text-3xl font-extrabold text-gray-900 dark:text-white">Request a Demo</h1>
          <p className="mt-4 text-lg text-gray-500 dark:text-gray-400">
            Join MediCore to manage your practice digitally. Please fill out the form below to get started.
          </p>
        </div>

        <Card>
          <CardContent className="p-8">
            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Name</label>
                  <Input name="fullName" value={formData.fullName} onChange={handleChange} required placeholder="Dr. Jane Doe" />
                </div>
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Email</label>
                  <Input type="email" name="email" value={formData.email} onChange={handleChange} required placeholder="doctor@example.com" />
                </div>
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Contact Number</label>
                  <Input name="phoneNumber" value={formData.phoneNumber} onChange={handleChange} required placeholder="+92 300 0000000" />
                </div>
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Specialization</label>
                  <Input name="specialization" value={formData.specialization} onChange={handleChange} required placeholder="Cardiology, General, etc." />
                </div>
                <div className="space-y-1 sm:col-span-2">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Current Practice Location / Hospital</label>
                  <Input name="clinicOrHospital" value={formData.clinicOrHospital} onChange={handleChange} required placeholder="Shifa International Hospital" />
                </div>
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">City</label>
                  <Input name="city" value={formData.city} onChange={handleChange} required placeholder="Islamabad" />
                </div>
                <div className="space-y-1">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Years of Experience</label>
                  <Input type="number" name="yearsOfExperience" value={formData.yearsOfExperience} onChange={handleChange} required min="0" />
                </div>
              </div>

              <div className="border-t border-gray-200 dark:border-slate-700 pt-6 mt-6">
                <h3 className="text-lg font-medium text-gray-900 dark:text-white mb-4">Verification Documents</h3>
                <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Degree Image</label>
                    <div className="flex items-center justify-center w-full">
                      <label className="flex flex-col items-center justify-center w-full h-32 border-2 border-dashed border-gray-300 dark:border-slate-600 rounded-lg cursor-pointer bg-gray-50 dark:bg-slate-800 hover:bg-gray-100 dark:hover:bg-slate-700">
                        <div className="flex flex-col items-center justify-center pt-5 pb-6">
                          <FileUp className="w-8 h-8 text-gray-400 mb-2" />
                          <p className="text-sm text-gray-500 dark:text-gray-400">
                            {degreeFile ? degreeFile.name : "Click to upload degree"}
                          </p>
                        </div>
                        <input type="file" className="hidden" accept="image/*,.pdf" onChange={(e) => handleFileChange(e, "degree")} required />
                      </label>
                    </div>
                  </div>

                  <div className="space-y-2">
                    <label className="text-sm font-medium text-gray-700 dark:text-gray-300">License Image</label>
                    <div className="flex items-center justify-center w-full">
                      <label className="flex flex-col items-center justify-center w-full h-32 border-2 border-dashed border-gray-300 dark:border-slate-600 rounded-lg cursor-pointer bg-gray-50 dark:bg-slate-800 hover:bg-gray-100 dark:hover:bg-slate-700">
                        <div className="flex flex-col items-center justify-center pt-5 pb-6">
                          <FileUp className="w-8 h-8 text-gray-400 mb-2" />
                          <p className="text-sm text-gray-500 dark:text-gray-400">
                            {licenseFile ? licenseFile.name : "Click to upload license"}
                          </p>
                        </div>
                        <input type="file" className="hidden" accept="image/*,.pdf" onChange={(e) => handleFileChange(e, "license")} required />
                      </label>
                    </div>
                  </div>
                </div>
              </div>

              <div className="pt-4">
                <Button type="submit" disabled={loading} className="w-full h-12 text-lg bg-[var(--color-primary-600)]">
                  {loading ? (
                    <>
                      <Loader2 className="w-5 h-5 mr-2 animate-spin" /> Submitting Request...
                    </>
                  ) : (
                    "Submit Demo Request"
                  )}
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
