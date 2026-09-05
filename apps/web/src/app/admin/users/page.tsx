"use client";

import { useEffect, useState } from "react";
import { AdminService } from "@medichp/api-client";
import { User } from "lucide-react";

interface UserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  isActive: boolean;
  role: string;
  createdAt: string;
}

export default function AdminUsersPage() {
  const [users, setUsers] = useState<UserDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [actionLoading, setActionLoading] = useState<string | null>(null);

  const loadUsers = async () => {
    try {
      setLoading(true);
      const res = await AdminService.getUsers();
      if (res?.success) {
        setUsers(res.data);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUsers();
  }, []);

  const toggleStatus = async (id: string, isActive: boolean) => {
    try {
      setActionLoading(id);
      const res = await AdminService.toggleUserStatus(id, isActive, isActive ? null : "Admin frozen");
      if (res?.success) {
        setUsers(users.map((u) => (u.id === id ? { ...u, isActive } : u)));
      }
    } catch (err) {
      console.error(err);
    } finally {
      setActionLoading(null);
    }
  };

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6 flex items-center gap-2">
        <User className="w-6 h-6" /> User Management
      </h1>

      {loading ? (
        <div>Loading users...</div>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="w-full text-left">
            <thead className="bg-slate-50 border-b">
              <tr>
                <th className="px-6 py-3 font-medium text-slate-500">Name</th>
                <th className="px-6 py-3 font-medium text-slate-500">Email</th>
                <th className="px-6 py-3 font-medium text-slate-500">Role</th>
                <th className="px-6 py-3 font-medium text-slate-500">Status</th>
                <th className="px-6 py-3 font-medium text-slate-500">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {users.map((u) => (
                <tr key={u.id} className="hover:bg-slate-50">
                  <td className="px-6 py-4">{u.firstName} {u.lastName}</td>
                  <td className="px-6 py-4">{u.email}</td>
                  <td className="px-6 py-4">
                    <span className="bg-blue-100 text-blue-700 px-2 py-1 rounded text-xs font-medium uppercase">
                      {u.role}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    {u.isActive ? (
                      <span className="bg-green-100 text-green-700 px-2 py-1 rounded text-xs font-medium">Active</span>
                    ) : (
                      <span className="bg-red-100 text-red-700 px-2 py-1 rounded text-xs font-medium">Frozen</span>
                    )}
                  </td>
                  <td className="px-6 py-4">
                    <button
                      disabled={actionLoading === u.id}
                      onClick={() => toggleStatus(u.id, !u.isActive)}
                      className={`px-3 py-1 text-sm font-medium rounded ${
                        u.isActive
                          ? "bg-red-50 text-red-600 hover:bg-red-100"
                          : "bg-green-50 text-green-600 hover:bg-green-100"
                      } ${actionLoading === u.id ? "opacity-50 cursor-not-allowed" : ""}`}
                    >
                      {actionLoading === u.id ? "Saving..." : u.isActive ? "Freeze" : "Unfreeze"}
                    </button>
                  </td>
                </tr>
              ))}
              {users.length === 0 && (
                <tr>
                  <td colSpan={5} className="px-6 py-4 text-center text-slate-500">
                    No users found.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
