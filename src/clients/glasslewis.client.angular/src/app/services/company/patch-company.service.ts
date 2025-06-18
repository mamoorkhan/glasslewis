import { protectedResources } from '../../auth-config';
import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PatchCompanyRequestModel } from '../../models/request/patch-company-request.model';
import { PatchCompanyResponseModel } from '../../models/response/patch-company-response.model';

@Injectable({
    providedIn: 'root'
})
export class PatchCompanyService {
    url = protectedResources.companyAPI.endpoint;

    http = inject(HttpClient);

    patchCompany(id: string, patchCompanyRequestModel: PatchCompanyRequestModel) {
        return this.http.patch<PatchCompanyResponseModel>(`${this.url}/${id}`, patchCompanyRequestModel);
    }
}