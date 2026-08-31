"use client";

import Link from "next/link";
import { UserPlus, Users } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent, CardHeader, CardTitle } from "@medichp/ui";

export default function DoctorPatientsPage() {
  return (
    <div className="space-y-6 max-w-5xl mx-auto pb-12">
      <div className="flex items-center justify-between border-b border-gray-200 dark:border-slate-800 pb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">My Patients</h1>
          <p className="text-gray-500">Manage and view your patients.</p>
        </div>
        <Button >
          <Link href="/doctor/patients/add">
            <UserPlus className="w-4 h-4 mr-2" />
            Add Patient
          </Link>
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2 text-gray-700 dark:text-gray-300">
            <Users className="w-5 h-5 text-indigo-500" />
            Patient Directory
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="text-center py-12">
            <Users className="w-12 h-12 text-gray-300 dark:text-gray-600 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-gray-900 dark:text-white">No patients found</h3>
            <p className="text-gray-500 mt-1 mb-6">You haven't added any patients yet or none are assigned to you.</p>
            <Button  variant="outline">
              <Link href="/doctor/patients/add">
                <UserPlus className="w-4 h-4 mr-2" />
                Add Your First Patient
              </Link>
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
