"use client";
import React, { useEffect } from 'react'
import axiosClientInstacne from '@/api/axiosClientInstance';
import domainApi from '@/api';


export default function AppInteractive() {
  const api = domainApi(axiosClientInstacne);
  console.log('chạy ở client');
  return (
    <div>
      component tương tác
    </div>
  )
}
