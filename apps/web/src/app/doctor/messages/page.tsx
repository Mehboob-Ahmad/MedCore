"use client";

import { motion } from "framer-motion";
import { Card, CardContent } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Search, Send, FileText } from "lucide-react";

export default function DoctorMessages() {
  const threads = [
    { id: 1, name: "Jane Doe", preview: "Thank you, doctor.", time: "10:30 AM", unread: true },
    { id: 2, name: "Mark Smith", preview: "When should I take the medicine?", time: "Yesterday", unread: false },
  ];

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
        <div className="flex-1 overflow-y-auto">
          {threads.map((thread) => (
            <div 
              key={thread.id} 
              className={`p-4 border-b border-gray-50 dark:border-slate-800/50 cursor-pointer transition-colors ${thread.unread ? 'bg-indigo-50 dark:bg-indigo-900/10' : 'hover:bg-slate-50 dark:hover:bg-slate-800/50'}`}
            >
              <div className="flex justify-between items-start mb-1">
                <span className={`font-semibold text-sm ${thread.unread ? 'text-gray-900 dark:text-white' : 'text-gray-700 dark:text-gray-300'}`}>
                  {thread.name}
                </span>
                <span className="text-xs text-gray-500">{thread.time}</span>
              </div>
              <p className={`text-sm truncate ${thread.unread ? 'text-gray-800 dark:text-gray-200 font-medium' : 'text-gray-500'}`}>
                {thread.preview}
              </p>
            </div>
          ))}
        </div>
      </Card>

      {/* Active Chat Area - Hidden on small mobile until a thread is clicked, shown on desktop */}
      <Card className="flex-1 hidden md:flex flex-col h-full">
        <div className="p-4 border-b border-gray-100 dark:border-slate-800 flex justify-between items-center">
          <div>
            <h3 className="font-semibold text-gray-900 dark:text-white">Jane Doe</h3>
            <p className="text-xs text-green-600 dark:text-green-400">Online</p>
          </div>
          <Button variant="outline" size="sm" className="flex items-center gap-2">
            <FileText className="w-4 h-4" />
            Medical History
          </Button>
        </div>
        
        <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-slate-50/50 dark:bg-slate-900/50">
          <div className="flex justify-end">
            <div className="bg-indigo-600 text-white p-3 rounded-2xl rounded-tr-sm max-w-[80%] shadow-sm">
              <p className="text-sm">Hi Jane, I've reviewed your latest blood work.</p>
              <span className="text-[10px] text-indigo-200 mt-1 block text-right">10:28 AM</span>
            </div>
          </div>
          <div className="flex justify-end">
            <div className="bg-indigo-600 text-white p-3 rounded-2xl rounded-tr-sm max-w-[80%] shadow-sm">
              <p className="text-sm">Your test results look good. Keep up the diet we discussed.</p>
              <span className="text-[10px] text-indigo-200 mt-1 block text-right">10:30 AM</span>
            </div>
          </div>
          <div className="flex justify-start">
            <div className="bg-white dark:bg-slate-800 border border-gray-100 dark:border-slate-700 p-3 rounded-2xl rounded-tl-sm max-w-[80%] shadow-sm">
              <p className="text-sm text-gray-800 dark:text-gray-200">Thank you, doctor.</p>
              <span className="text-[10px] text-gray-400 mt-1 block">10:30 AM</span>
            </div>
          </div>
        </div>

        <div className="p-4 border-t border-gray-100 dark:border-slate-800">
          <div className="flex gap-2">
            <Input placeholder="Type a message..." className="flex-1" />
            <Button size="icon" className="shrink-0 bg-indigo-600 hover:bg-indigo-700">
              <Send className="h-4 w-4" />
            </Button>
          </div>
        </div>
      </Card>
    </div>
  );
}
