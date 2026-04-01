import axiosServerInstance from "@/api/axiosServerInstance";
import domainApi from "@/api";
import AppInteractive from "@/app/app.interactive";
import axiosClientInstacne from '@/api/axiosClientInstance';
export default async function Home() {
  // Assuming you have an Axios instance available
  const axios = await axiosServerInstance();
  const a = await domainApi(axios).auth.login({
    username: "admin",
    password: "admin123"
  });
  const api = domainApi(axiosClientInstacne);
  console.log('chạy ở server');
  return (
    <div>
      <AppInteractive />
    </div>
  );
}
