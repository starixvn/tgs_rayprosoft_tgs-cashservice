{{/*
Expand the name of the chart.
*/}}
{{- define "cashservice-worker.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "cashservice-worker.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "cashservice-worker.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "cashservice-worker.labels" -}}
{{ include "cashservice-worker.selectorLabels" . }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "cashservice-worker.selectorLabels" -}}
app.kubernetes.io/name: {{ include "cashservice-worker.name" . }}
app: {{ include "cashservice-worker.name" . }}
tgs.kind: cash
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "cashservice-worker.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "cashservice-worker.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}
