"use client";

import { useEffect, useState } from "react";
import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Search, Send, FileText, Loader2, MessageSquare } from "lucide-react";
import { DoctorService } from "@medichp/api-client";

export default function DoctorMessages() {
  const [threads, setThreads] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchMessages = async () => {
      try {
        const res = await DoctorService.getMessages();
        if (res.success) {
          setThreads(res.data);
        }
      } catch (error) {
        console.error("Failed to fetch messages", error);
      } finally {
        setLoading(false);
      }
    };
    fetchMessages();
  }, []);

  return (
    <div className="h-[calc(100vh-120px)] md:h-full flex flex-col md:flex-row gap-4 pb-16 md:pb-0">
      {/* Threads List */}
      <Card className="w-full md:w-80 flex flex-col flex-shrink-0 h-1/2 md:h-full">
        <div className="p-4 border-b border-gray-100 dark:border-slate-800">
          <h2 className="font-bold text-lg mb-4 text-gray-900 dark:text-white">Messages</h2>
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
            <Input placeholder="Search patients..." className="pl-9 h-9" />
          </div>
        </div>
        
        {loading ? (
          <div className="flex justify-center items-center h-full">
            <Loader2 className="w-6 h-6 animate-spin text-gray-500" />
          </div>
        ) : threads.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-full text-gray-500 p-4 text-center">
            <MessageSquare className="w-8 h-8 mb-2 opacity-20" />
            <p className="text-sm">No messages found.</p>
          </div>
        ) : (
          <div className="flex-1 overflow-y-auto">
            {threads.map((thread) => (
              <div 
                key={thread.id} 
                className={`p-4 border-b border-gray-50 dark:border-slate-800/50 cursor-pointer transition-colors ${thread.unread ? 'bg-indigo-50 dark:bg-indigo-900/10' : 'hover:bg-slate-50 dark:hover:bg-slate-800/50'}`}
              >
                <div className="flex justify-between items-start mb-1">
                  <span className={`font-semibold text-sm ${thread.unread ? 'text-gray-900 dark:text-white' : 'text-gray-700 dark:text-gray-300'}`}>
                    {thread.name || thread.patientName}
                  </span>
                  <span className="text-xs text-gray-500">{thread.time || new Date(thread.updatedAt || Date.now()).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                </div>
                <p className={`text-sm truncate ${thread.unread ? 'text-gray-800 dark:text-gray-200 font-medium' : 'text-gray-500'}`}>
                  {thread.preview || thread.lastMessage}
                </p>
              </div>
            ))}
          </div>
        )}
      </Card>

      {/* Active Chat Area - Hidden on small mobile until a thread is clicked, shown on desktop */}
      <Card className="flex-1 hidden md:flex flex-col h-full">
        {!loading && threads.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-full text-gray-500">
            <MessageSquare className="w-12 h-12 mb-4 opacity-20" />
            <p>Select a conversation to start messaging</p>
          </div>
        ) : (
          <>
            <div className="p-4 border-b border-gray-100 dark:border-slate-800 flex justify-between items-center">
              <div>
                <h3 className="font-semibold text-gray-900 dark:text-white">Patient</h3>
                <p className="text-xs text-green-600 dark:text-green-400">Online</p>
              </div>
              <Button variant="outline" size="sm" className="flex items-center gap-2">
                <FileText className="w-4 h-4" />
                Medical History
              </Button>
            </div>
            
            <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-slate-50/50 dark:bg-slate-900/50">
              <div className="text-center text-xs text-gray-400 my-4">This is the start of your conversation.</div>
            </div>

            <div className="p-4 border-t border-gray-100 dark:border-slate-800">
              <div className="flex gap-2">
                <Input placeholder="Type a message..." className="flex-1" />
                <Button size="icon" className="shrink-0 bg-indigo-600 hover:bg-indigo-700">
                  <Send className="h-4 w-4" />
                </Button>
              </div>
            </div>
          </>
        )}
      </Card>
    </div>
  );
}
