import { NextRequest, NextResponse } from 'next/server'
import axios, { AxiosError } from 'axios'
import { cookies } from 'next/headers'

const IS_PRODUCTION = process.env.NODE_ENV === 'production'
const BACKEND_URL = process.env.API_URL ?? 'http://172.26.0.14:5000'
interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
}
function setCookies(res: NextResponse, data: TokenResponse) {
  res.cookies.set('accessToken', data.accessToken, {
    httpOnly: true,
    secure: IS_PRODUCTION,
    sameSite: 'lax',
    maxAge: data.expiresIn,
    path: '/',
  })
  res.cookies.set('refreshToken', data.refreshToken, {
    httpOnly: true,
    secure: IS_PRODUCTION,
    sameSite: 'lax',
    maxAge: 60 * 60 * 24 * 7,
    path: '/',
  })
}

type RouteContext = { params: Promise<{ path: string[] }> }

async function handler(req: NextRequest, { params }: RouteContext) {
  try {
    console.log('req', req);
    const { path } = await params
    const endpoint = '/api/' + path.join('/')

    const { searchParams } = new URL(req.url)

    let body: unknown
    if (!['GET', 'HEAD'].includes(req.method.toUpperCase())) {
      const text = await req.text()
      body = text ? JSON.parse(text) : undefined
    }

    const cookieStore = await cookies()
    const accessToken = cookieStore.get('accessToken')?.value

    if (endpoint === '/api/auth/logout') {
      const res = NextResponse.json(null, { status: 200 })
      res.cookies.delete('accessToken')
      res.cookies.delete('refreshToken')
      return res
    }

    const response = await axios.request({
      baseURL: BACKEND_URL,
      method: req.method,
      url: endpoint,
      data: body,
      params: searchParams.size > 0 ? Object.fromEntries(searchParams) : undefined,
      headers: {
        'Content-Type': 'application/json',
        ...(accessToken && { Authorization: `Bearer ${accessToken}` }),
      },
    })

    console.log('response', response);

    const hasBody = response.data !== undefined && response.data !== '' && response.data !== null
    const res = hasBody
      ? NextResponse.json(response.data, { status: response.status })
      : new NextResponse(null, { status: response.status })

    if (endpoint === '/api/auth/login' || endpoint === '/api/auth/refresh-token') {
      setCookies(res, response.data as TokenResponse)
    }

    return res
  } catch (error) {
    if (error instanceof AxiosError && error.response) {
      return NextResponse.json(error.response.data, { status: error.response.status })
    }
    console.log('error', error);
    return NextResponse.json({ message: 'Internal server error' }, { status: 500 })
  }
}

export const GET = handler
export const POST = handler
export const PUT = handler
export const PATCH = handler
export const DELETE = handler
