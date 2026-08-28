"use client";

import { useState } from "react";
import Link from "next/link";
import { motion } from "framer-motion";
import { Stethoscope, Mail, Lock } from "lucide-react";
import { Button } from "@medichp/ui";
import { Card, CardContent, CardFooter } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { useAuth } from "@/contexts/AuthContext";

export default function LoginPage() {
  const { login } = useAuth();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [formData, setFormData] = useState({
    email: "",
    password: ""
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      await login(formData);
    } catch (err: any) {
      setError(err.message || "Invalid credentials");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex-1 flex items-center justify-center p-4 bg-surface-50 dark:bg-slate-900 bg-[radial-gradient(ellipse_at_top_right,_var(--tw-gradient-stops))] from-sky-100/40 via-transparent to-transparent dark:from-sky-900/10">
      <motion.div 
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="w-full max-w-md my-12"
      >
        <div className="text-center mb-8">
          <Link href="/" className="inline-flex items-center space-x-2">
            <Stethoscope className="w-8 h-8 text-[var(--color-primary-600)]" />
            <span className="font-bold text-2xl tracking-tight text-[var(--color-primary-600)] dark:text-sky-400">
              MedicHp
            </span>
          </Link>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white mt-6">Welcome back</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-2">Log in to manage your health.</p>
        </div>

        <Card className="border-0 shadow-xl shadow-sky-900/5 dark:shadow-none dark:bg-slate-800/80">
          <CardContent className="pt-6">
            {error && (
              <div className="mb-4 p-3 rounded bg-red-50 text-red-600 text-sm border border-red-200">
                {error}
              </div>
            )}
            <form className="space-y-4" onSubmit={handleSubmit}>
              <div className="space-y-1">
                <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Email address</label>
                <div className="relative">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input type="email" name="email" value={formData.email} onChange={handleChange} placeholder="john@example.com" className="pl-10" required />
                </div>
              </div>
              
              <div className="space-y-1">
                <div className="flex justify-between items-center">
                  <label className="text-sm font-medium text-gray-700 dark:text-gray-300">Password</label>
                  <a href="#" className="text-sm font-medium text-[var(--color-primary-600)] hover:underline">Forgot password?</a>
                </div>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input type="password" name="password" value={formData.password} onChange={handleChange} placeholder="••••••••" className="pl-10" required />
                </div>
              </div>

              <Button type="submit" className="w-full mt-6" size="lg" disabled={loading}>
                {loading ? "Logging in..." : "Log In"}
              </Button>
            </form>
          </CardContent>
          <CardFooter className="flex justify-center border-t border-gray-100 dark:border-slate-700/50 pt-6">
            <p className="text-sm text-gray-600 dark:text-gray-400">
              Don't have an account?{" "}
              <Link href="/register" className="font-semibold text-[var(--color-primary-600)] hover:underline">
                Sign up
              </Link>
            </p>
          </CardFooter>
        </Card>
      </motion.div>
    </div>
  );
}
