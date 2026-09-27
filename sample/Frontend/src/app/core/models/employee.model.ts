export interface LookupItem { id: number; name: string; }

export interface Employee {
  employeeId: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  salary: number;
  dateOfBirth: string;
  joiningDate: string;
  gender: string;
  employmentType: string;
  isActive: boolean;
  departmentId: number;
  departmentName: string;
  skills: LookupItem[];
  profileImageUrl: string | null;
}

export interface EmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  salary: number;
  dateOfBirth: string;
  joiningDate: string;
  gender: string;
  employmentType: string;
  isActive: boolean;
  departmentId: number;
  skillIds: number[];
}

export interface EmployeeQuery {
  search?: string;
  departmentId?: number;
  skillId?: number;
  isActive?: boolean;
  employmentType?: string;
  sortBy: string;
  sortDirection: 'asc' | 'desc';
  page: number;
  pageSize: number;
}

export interface PagedResult<T> { items: T[]; page: number; pageSize: number; totalCount: number; totalPages: number; }
export interface DashboardSummary { totalEmployees: number; activeEmployees: number; departmentCount: number; skillCount: number; }
export interface DepartmentEmployeeCount { departmentId: number; departmentName: string; employeeCount: number; }
