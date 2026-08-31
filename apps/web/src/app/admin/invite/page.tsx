"use client";

import { useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { AuthService } from "@medichp/api-client";
import { ShieldAlert, UserPlus, Mail, Phone, User } from "lucide-react";

export default function InviteAdminPage() {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phoneNumber: ""
  });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");

    try {
      const res = await AuthService.inviteAdmin(formData);
      if (res.success) {
        setMessage("success: Admin invited successfully. They will receive an email shortly.");
        setFormData({ firstName: "", lastName: "", email: "", phoneNumber: "" });
      }
    } catch (err: any) {
      setMessage("error: " + (err.message || "Failed to invite admin."));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">Invite Administrator</h1>
        <p className="text-gray-500 dark:text-gray-400">Add a new system administrator with full access privileges.</p>
      </div>

      <Card className="border-red-100 dark:border-red-900/30">
        <CardHeader className="bg-red-50 dark:bg-red-900/10 border-b border-red-100 dark:border-red-900/30">
          <CardTitle className="flex items-center gap-2 text-red-700 dark:text-red-400">
            <ShieldAlert className="w-5 h-5" />
            Administrative Privileges
          </CardTitle>
        </CardHeader>
        <CardContent className="p-6">
          <p className="text-sm text-gray-600 dark:text-gray-400 mb-6">
            The invited user will have complete access to the MedicHp system, including patient records, doctor management, and system settings. Ensure you trust this individual before proceeding.
          </p>

          {message && (
            <div className={`p-4 mb-6 rounded-lg text-sm font-medium ${
              message.startsWith("success:") ? "bg-green-50 text-green-700 border border-green-200" : "bg-red-50 text-red-700 border border-red-200"
            }`}>
              {message.replace("success: ", "").replace("error: ", "")}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">First Name</label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                  <Input 
                    required 
                    className="pl-9" 
                    placeholder="John"
                    value={formData.firstName}
                    onChange={e => setFormData({...formData, firstName: e.target.value})}
                  />
                </div>
              </div>
              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Last Name</label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                  <Input 
                    required 
                    className="pl-9" 
                    placeholder="Doe"
                    value={formData.lastName}
                    onChange={e => setFormData({...formData, lastName: e.target.value})}
                  />
                </div>
              </div>
            </div>

            <div className="space-y-1">
              <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Email Address</label>
              <div className="relative">
                <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input 
                  required 
                  type="email" 
                  className="pl-9" 
                  placeholder="admin@medichp.com"
                  value={formData.email}
                  onChange={e => setFormData({...formData, email: e.target.value})}
                />
              </div>
            </div>

            <div className="space-y-1">
              <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Phone Number</label>
              <div className="relative">
                <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input 
                  required 
                  type="tel" 
                  className="pl-9" 
                  placeholder="+1 (555) 000-0000"
                  value={formData.phoneNumber}
                  onChange={e => setFormData({...formData, phoneNumber: e.target.value})}
                />
              </div>
            </div>

            <div className="pt-4">
              <Button type="submit" disabled={loading} className="w-full bg-red-600 hover:bg-red-700 text-white">
                <UserPlus className="w-4 h-4 mr-2" />
                {loading ? "Sending Invitation..." : "Invite Administrator"}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
