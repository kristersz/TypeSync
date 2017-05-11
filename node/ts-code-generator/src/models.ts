export interface ImportModel {
    names: string[];
    path: string;
}

export interface PropertyModel {
    name: string;
    type: string;
    isPrivate?: boolean;
    initialValue?: any;
}

export interface ParameterModel {
    name: string;
    type: string;
    isPrivate?: boolean;
}

export interface ConstructorModel {
    parameters: ParameterModel[];
}

export interface MethodModel {
    name: string;
    returnType: string;
    isHttpService: boolean;
    httpMethod: HttpMethod;
    parameters: ParameterModel[];
}

export interface ClassModel {
    name: string;
    baseClass: string;
    decorators: string[];
    typeParameters: string[];
    imports: ImportModel[];
    properties: PropertyModel[];
    constructorDef: ConstructorModel;
    methods: MethodModel[];
}


export interface EnumMemberModel {
    name: string;
    value: string;
}

export interface EnumModel {
    name: string;
    members: EnumMemberModel[];
}

export enum HttpMethod {
    Get,
    Post,
    Put,
    Patch,
    Delete
}
