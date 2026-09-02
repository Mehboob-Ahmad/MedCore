"use client";

import { useEffect, useState, useRef } from "react";
import { Card } from "@medichp/ui";
import { Input } from "@medichp/ui";
import { Button } from "@medichp/ui";
import { Search, Send, Loader2, MessageSquare, Paperclip, Mic, Image as ImageIcon, Video, StopCircle, ArrowLeft } from "lucide-react";
import { ChatService, AuthService } from "@medichp/api-client";

export default function DoctorMessages() {
  const [threads, setThreads] = useState<any[]>([]);
  const [loadingThreads, setLoadingThreads] = useState(true);
  
  const [activeConversationId, setActiveConversationId] = useState<string | null>(null);
  const [messages, setMessages] = useState<any[]>([]);
  const [loadingMessages, setLoadingMessages] = useState(false);
  const [me, setMe] = useState<any>(null);

  const [messageInput, setMessageInput] = useState("");
  const [sending, setSending] = useState(false);
  
  const [isRecording, setIsRecording] = useState(false);
  const mediaRecorderRef = useRef<MediaRecorder | null>(null);
  const audioChunksRef = useRef<Blob[]>([]);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const fetchMe = async () => {
      try {
        const res = await AuthService.getProfile();
        if (res.success) setMe(res.data);
      } catch (err) {}
    };
    fetchMe();
  }, []);

  useEffect(() => {
    fetchThreads();
    const interval = setInterval(fetchThreads, 10000);
    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    if (activeConversationId) {
      fetchMessages(activeConversationId);
      const interval = setInterval(() => {
        fetchMessages(activeConversationId, true);
      }, 5000);
      return () => clearInterval(interval);
    }
  }, [activeConversationId]);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  const fetchThreads = async () => {
    try {
      const res = await ChatService.getConversations();
      if (res.success) {
        setThreads(res.data);
      }
    } catch (error) {
      console.error("Failed to fetch conversations", error);
    } finally {
      setLoadingThreads(false);
    }
  };

  const fetchMessages = async (conversationId: string, background = false) => {
    if (!background) setLoadingMessages(true);
    try {
      const res = await ChatService.getMessages(conversationId);
      if (res.success) {
        setMessages(res.data);
      }
      
      // Mark as read
      const unreadCount = threads.find(t => t.id === conversationId)?.unreadCount || 0;
      if (unreadCount > 0) {
        await ChatService.markAsRead(conversationId);
        fetchThreads();
      }
    } catch (error) {
      console.error("Failed to fetch messages", error);
    } finally {
      setLoadingMessages(false);
    }
  };

  const handleSendMessage = async (e?: React.FormEvent) => {
    e?.preventDefault();
    if (!messageInput.trim() || !activeConversationId || sending) return;

    try {
      setSending(true);
      await ChatService.sendMessage(activeConversationId, {
        content: messageInput,
        messageType: "TEXT"
      });
      setMessageInput("");
      fetchMessages(activeConversationId);
      fetchThreads();
    } catch (error) {
      console.error("Failed to send message", error);
    } finally {
      setSending(false);
    }
  };

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file || !activeConversationId) return;

    try {
      setSending(true);
      const uploadRes = await ChatService.uploadChatMedia(file);
      if (uploadRes.success) {
        const type = file.type.startsWith("image/") ? "IMAGE" : "VIDEO";
        await ChatService.sendMessage(activeConversationId, {
          messageType: type,
          attachmentId: uploadRes.data.attachmentId
        });
        fetchMessages(activeConversationId);
        fetchThreads();
      }
    } catch (error) {
      console.error("Failed to upload media", error);
    } finally {
      setSending(false);
      if (fileInputRef.current) fileInputRef.current.value = "";
    }
  };

  const startRecording = async () => {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      const mediaRecorder = new MediaRecorder(stream);
      mediaRecorderRef.current = mediaRecorder;
      audioChunksRef.current = [];

      mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
          audioChunksRef.current.push(event.data);
        }
      };

      mediaRecorder.onstop = async () => {
        const audioBlob = new Blob(audioChunksRef.current, { type: 'audio/webm' });
        const file = new File([audioBlob], "voice_note.webm", { type: 'audio/webm' });
        
        try {
          setSending(true);
          const uploadRes = await ChatService.uploadChatMedia(file);
          if (uploadRes.success && activeConversationId) {
            await ChatService.sendMessage(activeConversationId, {
              messageType: "VOICE",
              attachmentId: uploadRes.data.attachmentId
            });
            fetchMessages(activeConversationId);
          }
        } catch (error) {
          console.error("Failed to send voice note", error);
        } finally {
          setSending(false);
        }
      };

      mediaRecorder.start();
      setIsRecording(true);
    } catch (error) {
      console.error("Microphone access denied or not available", error);
      alert("Microphone access is required to send voice notes.");
    }
  };

  const stopRecording = () => {
    if (mediaRecorderRef.current && isRecording) {
      mediaRecorderRef.current.stop();
      setIsRecording(false);
      mediaRecorderRef.current.stream.getTracks().forEach(t => t.stop());
    }
  };

  const activeThread = threads.find(t => t.id === activeConversationId);

  return (
    <div className="h-[calc(100vh-120px)] md:h-full flex flex-col md:flex-row gap-4 pb-16 md:pb-0">
      {/* Threads List */}
      <Card className={`w-full md:w-80 flex-col flex-shrink-0 h-full ${activeConversationId ? 'hidden md:flex' : 'flex'}`}>
        <div className="p-4 border-b border-gray-100 dark:border-slate-800">
          <h2 className="font-bold text-lg mb-4 text-gray-900 dark:text-white">Patient Messages</h2>
        </div>
        
        {loadingThreads ? (
          <div className="flex justify-center items-center h-full">
            <Loader2 className="w-6 h-6 animate-spin text-gray-500" />
          </div>
        ) : threads.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-full text-gray-500 p-4 text-center">
            <MessageSquare className="w-8 h-8 mb-2 opacity-20" />
            <p className="text-sm">No active conversations.</p>
          </div>
        ) : (
          <div className="flex-1 overflow-y-auto">
            {threads.map((thread) => (
              <div 
                key={thread.id} 
                onClick={() => setActiveConversationId(thread.id)}
                className={`p-4 border-b border-gray-50 dark:border-slate-800/50 cursor-pointer transition-colors ${activeConversationId === thread.id ? 'bg-primary-50 dark:bg-primary-900/20' : thread.unreadCount > 0 ? 'bg-sky-50 dark:bg-sky-900/10' : 'hover:bg-slate-50 dark:hover:bg-slate-800/50'}`}
              >
                <div className="flex justify-between items-start mb-1">
                  <span className={`font-semibold text-sm ${thread.unreadCount > 0 ? 'text-gray-900 dark:text-white' : 'text-gray-700 dark:text-gray-300'}`}>
                    {thread.otherParticipantName}
                  </span>
                  <span className="text-xs text-gray-500">
                    {thread.lastMessage ? new Date(thread.lastMessage.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) : ''}
                  </span>
                </div>
                <div className="flex justify-between items-center">
                  <p className={`text-sm truncate ${thread.unreadCount > 0 ? 'text-gray-800 dark:text-gray-200 font-medium' : 'text-gray-500'}`}>
                    {thread.lastMessage?.messageType === 'IMAGE' ? '📷 Image' : thread.lastMessage?.messageType === 'VIDEO' ? '🎥 Video' : thread.lastMessage?.messageType === 'VOICE' ? '🎤 Voice Note' : thread.lastMessage?.content || 'Started a conversation'}
                  </p>
                  {thread.unreadCount > 0 && (
                    <span className="bg-primary-500 text-white text-[10px] font-bold px-2 py-0.5 rounded-full">
                      {thread.unreadCount}
                    </span>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </Card>

      {/* Active Chat Area */}
      <Card className={`flex-1 flex-col h-full ${!activeConversationId ? 'hidden md:flex' : 'flex'}`}>
        {!activeConversationId ? (
          <div className="flex flex-col items-center justify-center h-full text-gray-500">
            <MessageSquare className="w-12 h-12 mb-4 opacity-20" />
            <p>Select a conversation to start messaging</p>
          </div>
        ) : (
          <>
            <div className="p-4 border-b border-gray-100 dark:border-slate-800 flex justify-between items-center bg-white dark:bg-slate-900 z-10 shadow-sm">
              <div className="flex items-center gap-2">
                <Button variant="ghost" size="icon" className="md:hidden mr-1" onClick={() => setActiveConversationId(null)}>
                  <ArrowLeft className="w-5 h-5" />
                </Button>
                <h3 className="font-semibold text-gray-900 dark:text-white">{activeThread?.otherParticipantName}</h3>
              </div>
            </div>
            
            <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-slate-50/50 dark:bg-slate-900/50 flex flex-col">
              {loadingMessages && messages.length === 0 ? (
                <div className="flex justify-center items-center h-full">
                  <Loader2 className="w-6 h-6 animate-spin text-gray-500" />
                </div>
              ) : messages.length === 0 ? (
                <div className="text-center text-xs text-gray-400 my-4">This is the start of your conversation.</div>
              ) : (
                messages.map((msg, i) => {
                  const isMine = msg.senderId === me?.id;
                  return (
                    <div key={msg.id || i} className={`flex ${isMine ? 'justify-end' : 'justify-start'}`}>
                      <div className={`max-w-[75%] rounded-2xl px-4 py-2 ${isMine ? 'bg-primary-600 text-white rounded-tr-sm' : 'bg-white dark:bg-slate-800 border border-gray-100 dark:border-slate-700 rounded-tl-sm'}`}>
                        {msg.messageType === 'TEXT' && <p className="text-sm">{msg.content}</p>}
                        
                        {msg.messageType === 'IMAGE' && msg.attachmentUrl && (
                          <a href={`https://medichp.onrender.com${msg.attachmentUrl}`} target="_blank" rel="noreferrer">
                            <img src={`https://medichp.onrender.com${msg.attachmentUrl}`} alt="Attachment" className="max-w-full rounded-lg mb-1 max-h-48 object-cover" />
                          </a>
                        )}
                        
                        {msg.messageType === 'VIDEO' && msg.attachmentUrl && (
                          <video src={`https://medichp.onrender.com${msg.attachmentUrl}`} controls className="max-w-full rounded-lg mb-1 max-h-48" />
                        )}

                        {msg.messageType === 'VOICE' && msg.attachmentUrl && (
                          <audio src={`https://medichp.onrender.com${msg.attachmentUrl}`} controls className="max-w-full h-10 mb-1" />
                        )}

                        <div className={`text-[10px] mt-1 flex justify-end gap-1 ${isMine ? 'text-primary-100' : 'text-gray-400'}`}>
                          {new Date(msg.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                          {isMine && (
                            <span>{msg.isRead ? '✓✓' : '✓'}</span>
                          )}
                        </div>
                      </div>
                    </div>
                  );
                })
              )}
              <div ref={messagesEndRef} />
            </div>

            <div className="p-3 md:p-4 border-t border-gray-100 dark:border-slate-800 bg-white dark:bg-slate-900">
              <form onSubmit={handleSendMessage} className="flex gap-2 items-center">
                <input 
                  type="file" 
                  accept="image/*,video/*" 
                  className="hidden" 
                  ref={fileInputRef} 
                  onChange={handleFileUpload} 
                />
                
                <Button 
                  type="button" 
                  variant="ghost" 
                  size="icon" 
                  className="shrink-0 text-gray-500 hover:text-primary-600"
                  onClick={() => fileInputRef.current?.click()}
                  disabled={sending}
                >
                  <Paperclip className="h-5 w-5" />
                </Button>
                
                {isRecording ? (
                  <Button 
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="shrink-0 text-red-500 animate-pulse"
                    onClick={stopRecording}
                  >
                    <StopCircle className="h-5 w-5" />
                  </Button>
                ) : (
                  <Button 
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="shrink-0 text-gray-500 hover:text-primary-600"
                    onClick={startRecording}
                    disabled={sending}
                  >
                    <Mic className="h-5 w-5" />
                  </Button>
                )}

                <Input 
                  value={messageInput}
                  onChange={(e) => setMessageInput(e.target.value)}
                  placeholder={isRecording ? "Recording..." : "Type a message..."} 
                  className="flex-1 bg-gray-50 dark:bg-slate-800 border-none" 
                  disabled={sending || isRecording}
                />
                
                <Button 
                  type="submit" 
                  size="icon" 
                  className="shrink-0 bg-primary-600 hover:bg-primary-700 text-white rounded-full h-10 w-10"
                  disabled={(!messageInput.trim() && !isRecording) || sending}
                >
                  {sending ? <Loader2 className="h-4 w-4 animate-spin" /> : <Send className="h-4 w-4 ml-1" />}
                </Button>
              </form>
            </div>
          </>
        )}
      </Card>
    </div>
  );
}
