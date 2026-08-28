"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { PatientService } from "@medichp/api-client";
import { 
  User, Calendar, MapPin, Activity, ShieldCheck, ArrowRight, CheckCircle 
} from "lucide-react";

export default function CompleteProfilePage() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [formData, setFormData] = useState({
    dateOfBirth: "",
    gender: "",
    bloodType: "",
    address: "",
    dataSharingConsent: false,
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      await PatientService.updateProfile({
        dateOfBirth: formData.dateOfBirth ? new Date(formData.dateOfBirth).toISOString() : null,
        gender: formData.gender,
        bloodType: formData.bloodType,
        address: formData.address,
        dataSharingConsent: formData.dataSharingConsent
      });
      setSuccess(true);
      setTimeout(() => {
        router.push("/patient/dashboard");
      }, 1500);
    } catch (err: any) {
      setError(err.message || "Failed to update profile");
    } finally {
      setLoading(false);
    }
  };

  if (success) {
    return (
      <div className="min-h-screen bg-surface-50 flex items-center justify-center p-4">
        <div className="bg-white rounded-2xl shadow-xl p-8 max-w-md w-full text-center space-y-4">
          <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto">
            <CheckCircle className="w-8 h-8 text-green-600" />
          </div>
          <h2 className="text-2xl font-bold text-gray-900">Profile Completed!</h2>
          <p className="text-gray-500">Redirecting to your dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-[calc(100vh-4rem)] bg-surface-50 py-12 px-4 sm:px-6 lg:px-8 flex flex-col justify-center">
      <div className="max-w-md w-full mx-auto space-y-8">
        <div className="text-center">
          <div className="w-16 h-16 bg-primary-100 rounded-full flex items-center justify-center mx-auto mb-4 shadow-sm">
            <User className="w-8 h-8 text-primary-600" />
          </div>
          <h2 className="text-3xl font-extrabold text-gray-900 tracking-tight">Complete Your Profile</h2>
          <p className="mt-2 text-sm text-gray-600">
            Please provide a few more details to help doctors provide better care.
          </p>
        </div>

        <div className="bg-white py-8 px-6 shadow-xl rounded-2xl sm:px-10 border border-gray-100">
          <form className="space-y-6" onSubmit={handleSubmit}>
            {error && (
              <div className="bg-red-50 text-red-600 p-3 rounded-lg text-sm border border-red-100">
                {error}
              </div>
            )}

            <div>
              <label htmlFor="dateOfBirth" className="block text-sm font-medium text-gray-700">Date of Birth</label>
              <div className="mt-1 relative rounded-md shadow-sm">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Calendar className="h-5 w-5 text-gray-400" />
                </div>
                <input
                  type="date"
                  name="dateOfBirth"
                  id="dateOfBirth"
                  required
                  value={formData.dateOfBirth}
                  onChange={(e) => setFormData({...formData, dateOfBirth: e.target.value})}
                  className="pl-10 block w-full rounded-xl border-gray-300 focus:ring-primary-500 focus:border-primary-500 sm:text-sm h-11 border transition-colors bg-surface-50"
                />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label htmlFor="gender" className="block text-sm font-medium text-gray-700">Gender</label>
                <div className="mt-1">
                  <select
                    id="gender"
                    name="gender"
                    required
                    value={formData.gender}
                    onChange={(e) => setFormData({...formData, gender: e.target.value})}
                    className="block w-full rounded-xl border-gray-300 focus:ring-primary-500 focus:border-primary-500 sm:text-sm h-11 border transition-colors bg-surface-50"
                  >
                    <option value="">Select</option>
                    <option value="Male">Male</option>
                    <option value="Female">Female</option>
                    <option value="Other">Other</option>
                  </select>
                </div>
              </div>

              <div>
                <label htmlFor="bloodType" className="block text-sm font-medium text-gray-700">Blood Type</label>
                <div className="mt-1 relative rounded-md shadow-sm">
                  <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    <Activity className="h-4 w-4 text-gray-400" />
                  </div>
                  <select
                    id="bloodType"
                    name="bloodType"
                    value={formData.bloodType}
                    onChange={(e) => setFormData({...formData, bloodType: e.target.value})}
                    className="pl-9 block w-full rounded-xl border-gray-300 focus:ring-primary-500 focus:border-primary-500 sm:text-sm h-11 border transition-colors bg-surface-50"
                  >
                    <option value="">Select</option>
                    <option value="A+">A+</option>
                    <option value="A-">A-</option>
                    <option value="B+">B+</option>
                    <option value="B-">B-</option>
                    <option value="O+">O+</option>
                    <option value="O-">O-</option>
                    <option value="AB+">AB+</option>
                    <option value="AB-">AB-</option>
                  </select>
                </div>
              </div>
            </div>

            <div>
              <label htmlFor="address" className="block text-sm font-medium text-gray-700">Address</label>
              <div className="mt-1 relative rounded-md shadow-sm">
                <div className="absolute inset-y-0 left-0 pl-3 pt-3 pointer-events-none">
                  <MapPin className="h-5 w-5 text-gray-400" />
                </div>
                <textarea
                  id="address"
                  name="address"
                  rows={3}
                  value={formData.address}
                  onChange={(e) => setFormData({...formData, address: e.target.value})}
                  className="pl-10 block w-full rounded-xl border-gray-300 focus:ring-primary-500 focus:border-primary-500 sm:text-sm py-3 border transition-colors bg-surface-50 resize-none"
                  placeholder="Enter your full address"
                />
              </div>
            </div>

            <div className="flex items-start bg-blue-50/50 p-4 rounded-xl border border-blue-100">
              <div className="flex items-center h-5">
                <input
                  id="dataSharingConsent"
                  name="dataSharingConsent"
                  type="checkbox"
                  checked={formData.dataSharingConsent}
                  onChange={(e) => setFormData({...formData, dataSharingConsent: e.target.checked})}
                  className="focus:ring-primary-500 h-4 w-4 text-primary-600 border-gray-300 rounded"
                />
              </div>
              <div className="ml-3 text-sm">
                <label htmlFor="dataSharingConsent" className="font-medium text-gray-700 flex items-center gap-1">
                  <ShieldCheck className="w-4 h-4 text-blue-500" />
                  Data Sharing Consent
                </label>
                <p className="text-gray-500 mt-1 leading-relaxed">I consent to sharing my medical data with registered doctors on the MedicHp platform for the purpose of my treatment.</p>
              </div>
            </div>

            <div>
              <button
                type="submit"
                disabled={loading}
                className="w-full flex justify-center items-center py-3 px-4 border border-transparent rounded-xl shadow-sm text-sm font-semibold text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 transition-colors disabled:opacity-70"
              >
                {loading ? "Saving..." : (
                  <>
                    Complete Profile
                    <ArrowRight className="ml-2 w-4 h-4" />
                  </>
                )}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
