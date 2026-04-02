import axiosServerInstance from "@/api/axiosServerInstance"
import mainApi from "@/api"
import ProfileForm from "./profile-form"

export default async function ProfilePage() {
  const axios = await axiosServerInstance()
  const { data: profile } = await mainApi(axios).auth.getProfile()()

  return (
    <div className="max-w-lg">
      <h1 className="text-2xl font-bold mb-6">Thông tin cá nhân</h1>
      <ProfileForm profile={profile} />
    </div>
  )
}
