"use client";

import { useEffect, useState } from "react";
import { AdminService } from "@medichp/api-client";
import { ClipboardList, ExternalLink, Loader2, Check, X } from "lucide-react";
import { Card, CardContent } from "@medichp/ui";
import { Button } from "@medichp/ui";

interface DemoRequestDto {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  specialization: string;
  city: string;
  clinicOrHospital: string;
  yearsOfExperience: number;
  degreeImageUrl?: string;
  licenseImageUrl?: string;
  status: number; // 0: Pending, 1: UnderReview, 2: Approved, 3: Rejected, 4: DemoCreated, 5: ConvertedToProduction
  createdAt: string;
}

export default function AdminDemoRequestsPage() {
  const [requests, setRequests] = useState<DemoRequestDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState<string | null>(null);

  const loadRequests = async () => {
    try {
      setLoading(true);
      const res = await AdminService.getDemoRequests();
      if (res?.success) {
        setRequests(res.data);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadRequests();
  }, []);

  const updateStatus = async (id: string, newStatus: number, notes?: string) => {
    try {
      setActionLoading(id);
      const res = await AdminService.updateDemoRequestStatus(id, { status: newStatus, notes });
      if (res?.success) {
        setRequests(requests.map((r) => (r.id === id ? { ...r, status: newStatus } : r)));
      }
    } catch (err) {
      console.error(err);
    } finally {
      setActionLoading(null);
    }
  };

  const createDoctor = async (id: string) => {
    try {
      setActionLoading(id);
      const res = await AdminService.createDoctorFromDemo(id);
      if (res?.success) {
        setRequests(requests.map((r) => (r.id === id ? { ...r, status: 5 } : r)));
        alert("Doctor account created successfully! The credentials have been emailed to the doctor.");
      }
    } catch (err: any) {
      console.error(err);
      alert(err?.message || "Failed to create doctor account");
    } finally {
      setActionLoading(null);
    }
  };

  const getStatusBadge = (status: number) => {
    switch (status) {
      case 0:
      case 1:
        return <span className="bg-yellow-100 text-yellow-800 px-2 py-1 rounded text-xs font-medium">Pending Review</span>;
      case 2:
      case 4:
      case 5:
        return <span className="bg-green-100 text-green-800 px-2 py-1 rounded text-xs font-medium">Approved</span>;
      case 3:
        return <span className="bg-red-100 text-red-800 px-2 py-1 rounded text-xs font-medium">Rejected</span>;
      default:
        return <span className="bg-gray-100 text-gray-800 px-2 py-1 rounded text-xs font-medium">Unknown</span>;
    }
  };

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6 flex items-center gap-2">
        <ClipboardList className="w-6 h-6" /> Demo Requests
      </h1>

      {loading ? (
        <div className="flex justify-center py-10">
          <Loader2 className="w-8 h-8 animate-spin text-[var(--color-primary-600)]" />
        </div>
      ) : requests.length === 0 ? (
        <Card>
          <CardContent className="p-10 text-center text-gray-500">
            No demo requests found.
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
          {requests.map((req) => (
            <Card key={req.id} className="overflow-hidden">
              <CardContent className="p-0">
                <div className="bg-gray-50 dark:bg-slate-800/50 p-4 border-b dark:border-slate-700 flex justify-between items-start">
                  <div>
                    <h3 className="font-bold text-lg text-gray-900 dark:text-white">{req.fullName}</h3>
                    <p className="text-sm text-gray-500">{req.email} • {req.phoneNumber}</p>
                  </div>
                  <div>{getStatusBadge(req.status)}</div>
                </div>
                
                <div className="p-4 space-y-4">
                  <div className="grid grid-cols-2 gap-4 text-sm">
                    <div>
                      <span className="block text-gray-500 font-medium">Specialization</span>
                      <span className="text-gray-900 dark:text-white">{req.specialization}</span>
                    </div>
                    <div>
                      <span className="block text-gray-500 font-medium">Experience</span>
                      <span className="text-gray-900 dark:text-white">{req.yearsOfExperience} years</span>
                    </div>
                    <div>
                      <span className="block text-gray-500 font-medium">Location</span>
                      <span className="text-gray-900 dark:text-white">{req.clinicOrHospital} ({req.city})</span>
                    </div>
                    <div>
                      <span className="block text-gray-500 font-medium">Submitted</span>
                      <span className="text-gray-900 dark:text-white">
                        {new Date(req.createdAt).toLocaleDateString()}
                      </span>
                    </div>
                  </div>

                  <div className="pt-4 border-t dark:border-slate-700 flex gap-4">
                    {req.degreeImageUrl && (
                      <a href={req.degreeImageUrl} target="_blank" rel="noreferrer" className="text-[var(--color-primary-600)] hover:underline flex items-center gap-1 text-sm">
                        <ExternalLink className="w-4 h-4" /> View Degree
                      </a>
                    )}
                    {req.licenseImageUrl && (
                      <a href={req.licenseImageUrl} target="_blank" rel="noreferrer" className="text-[var(--color-primary-600)] hover:underline flex items-center gap-1 text-sm">
                        <ExternalLink className="w-4 h-4" /> View License
                      </a>
                    )}
                  </div>

                  {(req.status === 0 || req.status === 1) && (
                    <div className="flex gap-3 pt-4">
                      <Button
                        onClick={() => updateStatus(req.id, 2, "Approved for production account")}
                        disabled={actionLoading === req.id}
                        className="flex-1 bg-green-600 hover:bg-green-700 text-white"
                      >
                        {actionLoading === req.id ? <Loader2 className="w-4 h-4 animate-spin" /> : <><Check className="w-4 h-4 mr-1" /> Approve</>}
                      </Button>
                      <Button
                        onClick={() => updateStatus(req.id, 3, "Rejected")}
                        disabled={actionLoading === req.id}
                        variant="outline"
                        className="flex-1 border-red-200 text-red-600 hover:bg-red-50"
                      >
                        {actionLoading === req.id ? <Loader2 className="w-4 h-4 animate-spin" /> : <><X className="w-4 h-4 mr-1" /> Reject</>}
                      </Button>
                    </div>
                  )}
                  {req.status === 2 && (
                    <div className="flex gap-3 pt-4">
                      <Button
                        onClick={() => createDoctor(req.id)}
                        disabled={actionLoading === req.id}
                        className="flex-1 bg-[var(--color-primary-600)] hover:opacity-90 text-white"
                      >
                        {actionLoading === req.id ? <Loader2 className="w-4 h-4 animate-spin" /> : <><Check className="w-4 h-4 mr-1" /> Create Doctor Account</>}
                      </Button>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
